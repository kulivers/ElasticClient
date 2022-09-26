using Processor;
using ProcessorsRunner;

public class SuperAgent
{
    private readonly ServicesConfig _servicesConfig;
    private readonly ConnectorsConfig _connectorsConfig;
    public IProcessorsContainer ProcessorsContainer { get; }
    public List<IConnector> Connectors { get; set; }

    public SuperAgent(AgentConfig agentConfig) : this(new ServicesConfig(agentConfig.Services), new ConnectorsConfig(agentConfig.Connectors))
    {
    }

    public SuperAgent(ServicesConfig servicesConfig, ConnectorsConfig connectorsConfig)
    {
        _servicesConfig = servicesConfig;
        _connectorsConfig = connectorsConfig;
        ProcessorsContainer = new ProcessorContainer(servicesConfig);
        InitConnectors(connectorsConfig);
        ThrowIfConfigsNotValid();
        CheckHealth();
    }

    private void InitConnectors(ConnectorsConfig connectorsConfig)
    {
        Connectors = new List<IConnector>();
        foreach (var connectorConfig in connectorsConfig.Connectors)
        {
            if (connectorConfig.IoServiceType == IoType.Kafka)
            {
                Connectors.Add(new KafkaConnector(connectorConfig));
            }
        }
    }

    public async Task Start()
    {
        await MapConnectors();
    }

    private async Task MapConnectors()
    {
        foreach (var connector in Connectors)
        {
            connector.OnReceive += (_, data) =>
            {
                var response = ProcessorsContainer.Process(connector.DestinationService, data);
            };
            await connector.StartReceive(CancellationToken.None);
        }
    }


    private void ThrowIfConfigsNotValid()
    {
        ValidateServicesConfig();
        ValidateConnectorsConfig();

        void ValidateServicesConfig()
        {
            foreach (var config in _servicesConfig.Services)
            {
                File.Open(config.Dll, FileMode.Open, FileAccess.Read).Dispose();
                File.Open(config.Config, FileMode.Open, FileAccess.Read).Dispose();
            }
        }

        void ValidateConnectorsConfig()
        {
            foreach (var config in _connectorsConfig.Connectors)
            {
                if (!_servicesConfig.Services.Any(cfg => cfg.ServiceName == config.Destination))
                    throw new ApplicationException(
                        $"There is no service {config.Destination} for {config.IoServiceType} connector");
                if (config.IoServiceType == IoType.Kafka)
                {
                    var path = config.Properties["config"];
                    File.Open(path, FileMode.Open, FileAccess.Read).Dispose();
                }
            }
        }
    }

    private void CheckHealth()
    {
        foreach (var processor in ProcessorsContainer)
        {
            processor.CheckHealth();
        }

        foreach (var connector in Connectors)
        {
            connector.CheckHealth();
        }
    }
}
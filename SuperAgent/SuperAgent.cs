using Localization.SuperAgent;
using Microsoft.VisualBasic;
using Processor;
using ProcessorsRunner;

public class SuperAgent
{
    private readonly ProcessorsConfig _processorsConfig;
    private readonly ConnectorsConfig _connectorsConfig;
    private readonly string NoServiceForConnector = SuperAgentResources.NoServiceForConnector;
    public IProcessorsContainer ProcessorsContainer { get; }
    public List<IConnector> Connectors { get; set; }

    public SuperAgent(AgentConfig agentConfig) : this(new ProcessorsConfig(agentConfig.Processors), new ConnectorsConfig(agentConfig.Connectors))
    {
    }

    public SuperAgent(ProcessorsConfig processorsConfig, ConnectorsConfig connectorsConfig)
    {
        _processorsConfig = processorsConfig;
        _connectorsConfig = connectorsConfig;
        ProcessorsContainer = new ProcessorContainer(processorsConfig);
        InitConnectors(connectorsConfig);
        ThrowIfConfigsNotValid();
        CheckHealth();
    }

    private void InitConnectors(ConnectorsConfig connectorsConfig)
    {
        Connectors = new List<IConnector>();
        var factory = new ConnectorFactory();
        foreach (var connectorConfig in connectorsConfig.Connectors)
        {
            if (connectorConfig.Input == InputService.Kafka)
            {
                Connectors.Add(factory.CreateConnector(connectorConfig));
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
                var response = ProcessorsContainer.Process(connector.DestinationProcessor, (string)data); 
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
            foreach (var config in _processorsConfig.Processors)
            {
                File.Open(config.Dll, FileMode.Open, FileAccess.Read).Dispose();
                File.Open(config.Config, FileMode.Open, FileAccess.Read).Dispose();
            }
        }

        void ValidateConnectorsConfig()
        {
            foreach (var connectorConfig in _connectorsConfig.Connectors)
            {
                if (!_processorsConfig.Processors.Any(cfg => cfg.Name == connectorConfig.Destination))
                {
                    throw new ApplicationException(Strings.Format(NoServiceForConnector, connectorConfig.Destination));
                }

                var inputConfig = connectorConfig.InputConfig;
                File.Open(inputConfig, FileMode.Open, FileAccess.Read).Dispose();

                var outputConfig = connectorConfig?.OutputConfig;
                if (outputConfig != null)
                {
                    File.Open(outputConfig, FileMode.Open, FileAccess.Read).Dispose();
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
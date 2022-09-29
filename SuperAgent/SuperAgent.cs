using Localization.SuperAgent;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Processor;
using ProcessorsRunner;

public class SuperAgent
{
    private readonly ProcessorsConfigs _processorsConfigs;
    private readonly ConnectorsConfig _connectorsConfig;
    private readonly string NoServiceForConnector = SuperAgentResources.NoServiceForConnector;
    public IProcessorsContainer ProcessorsContainer { get; }
    public List<IConnector> Connectors { get; set; }

    public SuperAgent(AgentConfig agentConfig) : this(new ProcessorsConfigs(agentConfig.Processors), new ConnectorsConfig(agentConfig.Connectors))
    {
    }

    public SuperAgent(ProcessorsConfigs processorsConfigs, ConnectorsConfig connectorsConfig)
    {
        _processorsConfigs = processorsConfigs;
        _connectorsConfig = connectorsConfig;
        ProcessorsContainer = new ProcessorContainer(processorsConfigs);
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

        foreach (var connector in Connectors)
        {
            connector.OnReceive += (_, data) =>
            {
                var destinationProcessor = connector.DestinationProcessor;
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                var response = ProcessorsContainer.Process(destinationProcessor, data, cts.Token);
                if (response != null)
                {
                    connector.SendToOutputService(response);
                };
            };
        }
    }

    public async Task Start()
    {
        foreach (var connector in Connectors)
        {
            connector.StartReceive(CancellationToken.None);
        }
    }

    private void ThrowIfConfigsNotValid()
    {
        ValidateServicesConfig();
        ValidateConnectorsConfig();

        void ValidateServicesConfig()
        {
            foreach (var config in _processorsConfigs.Processors)
            {
                File.Open(config.Dll, FileMode.Open, FileAccess.Read).Dispose();
                File.Open(config.Config, FileMode.Open, FileAccess.Read).Dispose();
            }
        }

        void ValidateConnectorsConfig()
        {
            foreach (var connectorConfig in _connectorsConfig.Connectors)
            {
                if (!_processorsConfigs.Processors.Any(cfg => cfg.Name == connectorConfig.Destination))
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
using System.Reflection;
using Confluent.Kafka;
using Newtonsoft.Json;
using Processor;
using ProcessorsRunner;


public class SuperAgent
{
    public IProcessorsContainer ProcessorsContainer { get; }
    public List<IConnector> Connectors { get; set; }

    public SuperAgent(AgentConfig agentConfig) : this(new ServicesConfig(agentConfig.Services),
        new ConnectorsConfig(agentConfig.Connectors))
    {
    }

    public SuperAgent(ServicesConfig servicesConfig, ConnectorsConfig connectorsConfig)
    {
        ProcessorsContainer = new ProcessorContainer(servicesConfig);
        InitConnectors(connectorsConfig);
    }

    public void Start()
    {
        MapConnectors();
    }

    private void InitConnectors(ConnectorsConfig connectorsConfig)
    {
        Connectors = new List<IConnector>();
        foreach (var connectorConfig in connectorsConfig.Connectors)
        {
            if (connectorConfig.IoServiceType == IoType.Kafka) Connectors.Add(new KafkaConnector(connectorConfig));
        }
    }

    private void MapConnectors()
    {
        foreach (var connector in Connectors)
        {
            MapConnector(connector);
        }
    }

    private void MapConnector(IConnector connector)
    {
        connector.OnReceive += (_, data) => { ProcessorsContainer.Process(connector.DestinationService, data); };
        connector.StartReceive(CancellationToken.None);
    }
}
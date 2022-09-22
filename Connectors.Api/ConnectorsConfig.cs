public class ConnectorsConfig
{
    public IEnumerable<ConnectorConfig> Connectors { get; set; }

    public ConnectorsConfig(IEnumerable<ConnectorConfig> connectors)
    {
        Connectors = connectors;
    }
}

public class ConnectorConfig
{
    public string Destination { get; set; }
    public IoType IoServiceType { get; set; }
    public Dictionary<string, string>? Properties { get; set; }
}


public enum IoType
{
    Kafka
}
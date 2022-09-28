public class ConnectorsConfig
{
    //for config parsing
    public IEnumerable<ConnectorConfig> Connectors { get; set; }

    public ConnectorsConfig(IEnumerable<ConnectorConfig> connectors)
    {
        Connectors = connectors;
    }
}

public class ConnectorConfig
{
    public string Destination { get; set; }
    public InputService Input { get; set; }
    public string InputConfig { get; set; }
    public OutputService? Output { get; set; }
    public string? OutputConfig { get; set; }
}


public enum InputService
{
    Kafka
}

public enum OutputService
{
    Kafka
}
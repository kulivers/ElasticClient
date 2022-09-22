using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class ConnectorsConfig
{
    public IEnumerable<ConnectorConfig> Connectors { get; set; }

    public ConnectorsConfig()
    {
        
    }

    public ConnectorsConfig(IEnumerable<ConnectorConfig> connectors)
    {
        Connectors = connectors;
    }
    public static ConnectorsConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
            throw new ArgumentException("wrong type of file. need to be .yaml");
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var fileContent = File.ReadAllText(path);
        return deserializer.Deserialize<ConnectorsConfig>(fileContent);
    }
}

public class ConnectorConfig
{
    public string Destination { get; set; }
    public IoType IoServiceType { get; set; }
    public Dictionary<string, string> Properties { get; set; }

    public static ConnectorConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
            throw new ArgumentException("wrong type of file. need to be .yaml");
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var fileContent = File.ReadAllText(path);
        return deserializer.Deserialize<ConnectorConfig>(fileContent);
    }
}


public enum IoType
{
    Kafka
}
using Processor;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ProcessorsRunner;

public class AgentConfig
{
    public IEnumerable<ServiceConfig> Services { get; set; } 
    public IEnumerable<ConnectorConfig> Connectors { get; set; }
    
    public static AgentConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
        {
            throw new ArgumentException("wrong type of file. need to be .yaml");
        }
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) //todo egor mb it is not camel case
            .Build();
        
        var fileContent = File.ReadAllText(path);
        return deserializer.Deserialize<AgentConfig>(fileContent);
    }

}
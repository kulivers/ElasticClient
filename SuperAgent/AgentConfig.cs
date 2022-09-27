using Localization.SuperAgent;
using Processor;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ProcessorsRunner;

public class AgentConfig
{
    private static readonly string WrongTypeOfFileNeedToBeYaml = SuperAgentResources.WrongTypeOfFileNeedToBeYaml;
    public IEnumerable<ProcessorConfig> Processors { get; set; } 
    public IEnumerable<ConnectorConfig> Connectors { get; set; }
    
    public static AgentConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
        {
            throw new ArgumentException(WrongTypeOfFileNeedToBeYaml);
        }
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) 
            .Build();
        
        var fileContent = File.ReadAllText(path);
        return deserializer.Deserialize<AgentConfig>(fileContent);
    }

}
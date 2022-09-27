using Processor;
using Utils;

namespace ProcessorsRunner;

public class AgentConfig
{
    public IEnumerable<ProcessorConfig> Processors { get; set; } 
    public IEnumerable<ConnectorConfig> Connectors { get; set; }
    
    public static AgentConfig FromYaml(string path)
    {
        return YamlConfigHelper.GetConfigFromCamelYaml<AgentConfig>(path);
    }

}
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Processor;

public class ProcessorsConfig
{
    public IEnumerable<ProcessorConfig> Processors { get; set; }

    public ProcessorsConfig()
    {
        
    }

    public ProcessorsConfig(IEnumerable<ProcessorConfig> processors)
    {
        Processors = processors;
    }

    public static ProcessorsConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
        {
            throw new ArgumentException("wrong type of file, need to be .yaml");
        }
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) //todo egor mb it is not camel case
            .Build();
        var fileContent = File.ReadAllText(path);
        return deserializer.Deserialize<ProcessorsConfig>(fileContent);
    }
}

public class ProcessorConfig
{
    public string Dll { get; set; }
    public string Config { get; set; }
    public string Name { get; set; }

    public ConfigType ConfigType
    {
        get
        {
            if (Config.EndsWith("yaml")) return ConfigType.Yaml;
            if (Config.EndsWith("json")) return ConfigType.Json;
            if (Config.EndsWith("txt")) return ConfigType.Text;
            if (Config.EndsWith("xml")) return ConfigType.Xml;
            throw new NotImplementedException("Unsupported type of config");
        }
    }

    public override bool Equals(object? otherObj)
    {
        if (otherObj is not ProcessorConfig other)
            return false;
        return Dll == other.Dll && Config == other.Config && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Dll, Config, Name);
    }
}

public enum ConfigType
{
    Yaml,
    Json,
    Xml,
    Text
}
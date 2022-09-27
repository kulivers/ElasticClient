using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Processor;

public class ProcessorsConfig
{
    private const string? NotSupportedConfigType = "Not supported type of config file.";
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
            throw new NotSupportedException(NotSupportedConfigType);
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
    private const string? NotSupportedConfigType = "Not supported type of config file.";
    public string Dll { get; set; }
    public string Config { get; set; }
    public string Name { get; set; }

    public ConfigType ConfigType
    {
        get
        {
            if (Config.EndsWith("yaml"))
            {
                return ConfigType.Yaml;
            }
            throw new NotImplementedException(NotSupportedConfigType);
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
}
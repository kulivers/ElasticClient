using Localization.Processors;

namespace Processor;

public class ProcessorsConfigs
{
    //for config file
    public IEnumerable<ProcessorConfig> Processors { get; set; }

    public ProcessorsConfigs(IEnumerable<ProcessorConfig> processors)
    {
        Processors = processors;
    }
}

public class ProcessorConfig
{
    private static readonly string? NotSupportedConfigType = ProcessorsResources.NotSupportedConfigType;
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
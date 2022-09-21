using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Processor;

public class ServicesConfig
{
    public IEnumerable<ServiceConfig> Services { get; set; }

    public ServicesConfig()
    {
        
    }

    public ServicesConfig(IEnumerable<ServiceConfig> services)
    {
        Services = services;
    }

    public static ServicesConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
            throw new ArgumentException("wrong type of file, need to be .yaml");
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) //todo egor mb it is not camel case
            .Build();
        var fileContent = File.ReadAllText(path);
        return deserializer.Deserialize<ServicesConfig>(fileContent);
    }
}

public class ServiceConfig
{
    public string Dll { get; set; }
    public string Config { get; set; }
    public string ServiceName { get; set; }

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

    public static ServiceConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
            throw new ArgumentException("wrong type of file. need to be .yaml");
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) //todo egor mb it is not camel case
            .Build();
        var fileContent = File.ReadAllText(path);
        
        return deserializer.Deserialize<ServiceConfig>(fileContent);
    }

    public override bool Equals(object? otherObj)
    {
        if (otherObj is not ServiceConfig other)
            return false;
        return Dll == other.Dll && Config == other.Config && ServiceName == other.ServiceName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Dll, Config, ServiceName);
    }
}

public enum ConfigType
{
    Yaml,
    Json,
    Xml,
    Text
}
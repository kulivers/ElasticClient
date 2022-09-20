namespace Processor;

public class ServicesConfig
{
    public IEnumerable<ServiceConfig> Services { get; set; }
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

}

public enum ConfigType
{
    Yaml,
    Json,
    Xml,
    Text
}
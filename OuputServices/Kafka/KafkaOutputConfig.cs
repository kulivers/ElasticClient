using Confluent.Kafka;
using Localization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace InputServices;

public class KafkaOutputConfig
{
    private static readonly string WrongTypeOfFileNeedToBeYaml = IOServicesRecources.WrongTypeOfFileNeedToBeYaml;
    public IEnumerable<string> Topics { get; set; }
    public ClientConfig Client { get; set; }

    public KafkaOutputConfig()
    {
        
    }
    public KafkaOutputConfig(string path)
    {
        var config = FromYaml(path);
        Client = config.Client;
        Topics = config.Topics;
    }
    public static KafkaOutputConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
        {
            throw new ArgumentException(WrongTypeOfFileNeedToBeYaml);
        }

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) 
            .Build();
        var fileContent = File.ReadAllText(path);
        return deserializer.Deserialize<KafkaOutputConfig>(fileContent);
    }
    
}
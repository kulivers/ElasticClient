using Confluent.Kafka;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace InputServices;

public class KafkaInputConfig
{
    public IEnumerable<string> Topics { get; set; }
    public ClientConfig Client { get; set; }

    public KafkaInputConfig()
    {
        
    }
    public KafkaInputConfig(string path)
    {
        var config = FromYaml(path);
        Client = config.Client;
        Topics = config.Topics;
    }
    public static KafkaInputConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
        {
            throw new ArgumentException("wrong type of file. need to be .yaml");
        }

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) //todo egor mb it is not camel case
            .Build();
        var fileContent = File.ReadAllText(path);
        return deserializer.Deserialize<KafkaInputConfig>(fileContent);
    }
    
}
using Confluent.Kafka;
using Utils;

namespace InputServices;

public class KafkaInputConfig
{
    public IEnumerable<string> Topics { get; set; }
    public ClientConfig Client { get; set; }

    public KafkaInputConfig()
    {
        //for mapping from config file
    }
    public KafkaInputConfig(string path)
    {
        var config = FromYaml(path);
        Client = config.Client;
        Topics = config.Topics;
    }
    public static KafkaInputConfig FromYaml(string path)
    {
        return YamlConfigHelper.GetConfigFromCamelYaml<KafkaInputConfig>(path);
    }
    
}
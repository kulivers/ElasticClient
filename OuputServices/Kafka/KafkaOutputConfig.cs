using Confluent.Kafka;
using Utils;

namespace InputServices;

public class KafkaOutputConfig
{
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
        return YamlConfigHelper.GetConfigFromCamelYaml<KafkaOutputConfig>(path);
    }
    
}
using Confluent.Kafka;
using Utils;

namespace KafkaInteractor
{
    public class ConsumerConfigFactory
    {
        private const string DefaultGroupId = "foo";
        private ClientConfig ClientConfig { get; }

        private static ClientConfig FromYaml(string path)
        {
            return YamlConfigHelper.GetConfigFromCamelYaml<ClientConfig>(path);
        }

        public ConsumerConfigFactory(ClientConfig clientConfig)
        {
            ClientConfig = clientConfig;
        }

        public ConsumerConfig GetDefaultConsumerConfig()
        {
            var consumerConfig = new ConsumerConfig(ClientConfig);
            consumerConfig.GroupId ??= DefaultGroupId;
            consumerConfig.AutoOffsetReset ??= AutoOffsetReset.Latest;
            consumerConfig.EnableAutoCommit = false;
            return consumerConfig;
        }
    }
}
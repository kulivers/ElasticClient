using System.Net;
using Confluent.Kafka;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KafkaInteractor
{
    public class KafkaConfigFactory
    {
        public ClientConfig ClientConfig1 { get; }

        private static ClientConfig FromYaml(string path)
        {
            if (!path.EndsWith(".yaml"))
                throw new ArgumentException("wrong type of file. need to be .yaml");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) //todo egor mb it is not camel case
                .Build();
            var fileContent = File.ReadAllText(path);
            return deserializer.Deserialize<ClientConfig>(fileContent);
        }

        public KafkaConfigFactory(string path)
        {
            ClientConfig1 = FromYaml(path);
        }

        public KafkaConfigFactory(ClientConfig clientConfig)
        {
            ClientConfig1 = clientConfig;
        }

        public ProducerConfig GetDefaultProducerConfig()
        {
            var producerConfig = new ProducerConfig(ClientConfig1);
            producerConfig.ClientId ??= Dns.GetHostName();
            producerConfig.Partitioner ??= Partitioner.Consistent;
            producerConfig.Acks ??= Acks.All;
            return producerConfig;
        }

        public ConsumerConfig GetDefaultConsumerConfig()
        {
            var consumerConfig = new ConsumerConfig(ClientConfig1);
            consumerConfig.GroupId ??= "foo";
            consumerConfig.AutoOffsetReset ??= AutoOffsetReset.Latest;
            return consumerConfig;
        }
    }
}
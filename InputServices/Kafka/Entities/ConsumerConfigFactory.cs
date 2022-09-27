using Confluent.Kafka;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace KafkaInteractor
{
    public class ConsumerConfigFactory
    {
        private const string WrongTypeOfFileNeedToBeYaml = "wrong type of file. need to be .yaml";
        private const string DefaultGroupId = "foo";
        private ClientConfig ClientConfig { get; }

        private static ClientConfig FromYaml(string path)
        {
            if (!path.EndsWith(".yaml"))
            {
                throw new ArgumentException(WrongTypeOfFileNeedToBeYaml);
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) //todo egor mb it is not camel case
                .Build();
            var fileContent = File.ReadAllText(path);
            return deserializer.Deserialize<ClientConfig>(fileContent);
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

            return consumerConfig;
        }
    }
}
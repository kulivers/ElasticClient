using Confluent.Kafka;
using Localization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace KafkaInteractor
{
    public class ConsumerConfigFactory
    {
        private static readonly string WrongTypeOfFileNeedToBeYaml = IOServicesRecources.WrongTypeOfFileNeedToBeYaml;
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
using System.Net;
using Confluent.Kafka;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OuputServices.Kafka.Entities
{
    public class ProducerConfigFactory
    {
        private const string NoBootstrapServerSpecified = "No bootstrap server specified";
        private const string WrongTypeOfFileNeedToBeYaml = "wrong type of file. need to be .yaml";
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

        public ProducerConfigFactory(string path) : this(FromYaml(path))
        {
        }

        public ProducerConfigFactory(ClientConfig clientConfig)
        {
            ClientConfig = clientConfig;
        }

        public ProducerConfig GetDefaultProducerConfig()
        {
            if (ClientConfig.BootstrapServers == null)
            {
                throw new Exception(NoBootstrapServerSpecified);
            }

            ClientConfig.Acks ??= Acks.All;
            ClientConfig.ClientId ??= Dns.GetHostName();

            var producerConfig = new ProducerConfig(ClientConfig)
            {
                Partitioner = Partitioner.Consistent,
                QueueBufferingMaxMessages = 10000000
            };
            return producerConfig;
        }
    }
}
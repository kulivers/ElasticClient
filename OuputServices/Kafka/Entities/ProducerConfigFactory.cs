using System.Net;
using Confluent.Kafka;
using Localization;
using Utils;

namespace OuputServices.Kafka.Entities
{
    public class ProducerConfigFactory
    {
        private static readonly string NoBootstrapServerSpecified = IOServicesRecources.NoBootstrapServerSpecified;
        private ClientConfig ClientConfig { get; }

        private static ClientConfig FromYaml(string path)
        {
            return YamlConfigHelper.GetConfigFromCamelYaml<ClientConfig>(path);
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
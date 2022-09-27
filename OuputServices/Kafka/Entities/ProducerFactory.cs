using Confluent.Kafka;
using OuputServices.Kafka.Entities;

namespace KafkaInteractor
{
    
    public class ProducerFactory 
    {
        public ProducerConfig ProducerConfig { get; }

        public ProducerFactory(ProducerConfig producerConfig)
        {
            ProducerConfig = producerConfig;
        }

        public ProducerFactory(string configPath)
        {
            ProducerConfig = new ProducerConfigFactory(configPath).GetDefaultProducerConfig();
        }
        
        public ProducerFactory(ClientConfig config)
        {
            ProducerConfig = new ProducerConfigFactory(config).GetDefaultProducerConfig();
        }
        
        public IProducer<int, string> CreateStringProducer()
        {
            var producerBuilder = new ProducerBuilder<int, string>(ProducerConfig);
            var valueSerializer = new StringSerializer();
            return producerBuilder.SetValueSerializer(valueSerializer).Build();
        }
    }

}
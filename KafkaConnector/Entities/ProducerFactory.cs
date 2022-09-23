using Confluent.Kafka;
using KafkaInteractor;

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
            ProducerConfig = new KafkaConfigFactory(configPath).GetDefaultProducerConfig();
        }
        
        public ProducerFactory(ClientConfig config)
        {
            ProducerConfig = new KafkaConfigFactory(config).GetDefaultProducerConfig();
        }
        
        public IProducer<int, string> CreateStringProvider()
        {
            var producerBuilder = new ProducerBuilder<int, string>(ProducerConfig);
            var valueSerializer = new StringSerializer();
            return producerBuilder.SetValueSerializer(valueSerializer).Build();
        }
    }

}
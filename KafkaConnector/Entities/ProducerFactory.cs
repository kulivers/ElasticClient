using Confluent.Kafka;
using KafkaInteractor;

namespace KafkaInteractor
{
    
    public class ProducerFactory 
    {
        private readonly ProducerConfig _producerConfig;

        public ProducerFactory(ProducerConfig producerConfig)
        {
            _producerConfig = producerConfig;
        }

        public ProducerFactory(string configPath)
        {
            _producerConfig = new KafkaConfigFactory(configPath).GetDefaultProducerConfig();
        }
        
        public ProducerFactory(ClientConfig config)
        {
            _producerConfig = new KafkaConfigFactory(config).GetDefaultProducerConfig();
        }
        
        public IProducer<int, string> CreateStringProvider() =>
            new ProducerBuilder<int,string>(_producerConfig).SetValueSerializer(new Serializers.StringSerializer()).Build();
    }

}
using Confluent.Kafka;
using KafkaInteractor;

namespace KafkaInteractor
{
    
    public class ProducerFactory 
    {
        public ProducerConfig ProducerConfig1 { get; }

        public ProducerFactory(ProducerConfig producerConfig)
        {
            ProducerConfig1 = producerConfig;
        }

        public ProducerFactory(string configPath)
        {
            ProducerConfig1 = new KafkaConfigFactory(configPath).GetDefaultProducerConfig();
        }
        
        public ProducerFactory(ClientConfig config)
        {
            ProducerConfig1 = new KafkaConfigFactory(config).GetDefaultProducerConfig();
        }
        
        public IProducer<int, string> CreateStringProvider() =>
            new ProducerBuilder<int,string>(ProducerConfig1).SetValueSerializer(new Serializers.StringSerializer()).Build();
    }

}
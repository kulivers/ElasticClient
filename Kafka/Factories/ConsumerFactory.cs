using System.Text;
using Confluent.Kafka;

namespace KafkaInteractor
{
    
    public class ConsumerFactory 
    {
        private readonly ConsumerConfig _consumerConfig;

        public ConsumerFactory(ConsumerConfig consumerConfig)
        {
            _consumerConfig = consumerConfig;
        }

        public ConsumerFactory(string configPath)
        {
            _consumerConfig = new KafkaConfigFactory(configPath).GetDefaultConsumerConfig();
        }
        
        public ConsumerFactory(ClientConfig config)
        {
            _consumerConfig = new KafkaConfigFactory(config).GetDefaultConsumerConfig();
        }

        
        public IConsumer<int, string> CreateStringConsumer() =>
            new ConsumerBuilder<int, string>(_consumerConfig).SetValueDeserializer(new Deserializers.StringDeserializer()).Build();
    }
}
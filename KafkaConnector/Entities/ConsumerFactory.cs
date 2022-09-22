using System.Text;
using Confluent.Kafka;

namespace KafkaInteractor
{
    
    public class ConsumerFactory 
    {
        public ConsumerConfig ConsumerConfig { get; }

        public ConsumerFactory(ConsumerConfig consumerConfig)
        {
            ConsumerConfig = consumerConfig;
        }

        public ConsumerFactory(string configPath)
        {
            ConsumerConfig = new KafkaConfigFactory(configPath).GetDefaultConsumerConfig();
        }
        
        public ConsumerFactory(ClientConfig config)
        {
            ConsumerConfig = new KafkaConfigFactory(config).GetDefaultConsumerConfig();
        }

        
        public IConsumer<int, string> CreateStringConsumer() =>
            new ConsumerBuilder<int, string>(ConsumerConfig).SetValueDeserializer(new Deserializers.StringDeserializer()).Build();
    }
}
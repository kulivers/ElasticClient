using System.Text;
using Confluent.Kafka;

namespace KafkaInteractor
{
    
    public class ConsumerFactory 
    {
        private ConsumerConfig ConsumerConfig { get; }

        public ConsumerFactory(ConsumerConfig consumerConfig)
        {
            ConsumerConfig = consumerConfig;
        }

        public ConsumerFactory(string configPath)
        {
            var kafkaConfigFactory = new KafkaConfigFactory(configPath);
            ConsumerConfig = kafkaConfigFactory.GetDefaultConsumerConfig();
        }
        
        public ConsumerFactory(ClientConfig config)
        {
            var kafkaConfigFactory = new KafkaConfigFactory(config);
            ConsumerConfig = kafkaConfigFactory.GetDefaultConsumerConfig();
        }

        
        public IConsumer<int, string> CreateStringConsumer()
        {
            var builder = new ConsumerBuilder<int, string>(ConsumerConfig);
            var valueDeserializer = new StringDeserializer();
            return builder.SetValueDeserializer(valueDeserializer).Build();
        }
    }
}
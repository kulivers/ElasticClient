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

        public ConsumerFactory(ClientConfig config)
        {
            var kafkaConfigFactory = new ConsumerConfigFactory(config);
            ConsumerConfig = kafkaConfigFactory.GetDefaultConsumerConfig();
        }

        
        public IConsumer<int, string> CreateStringConsumer()
        {
            var builder = new ConsumerBuilder<int, string>(ConsumerConfig);
            var deserializer = new StringDeserializer();
            return builder.SetValueDeserializer(deserializer).Build();
        }
    }
}
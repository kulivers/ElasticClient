using System.Net;
using Confluent.Kafka;
using KafkaInteractor;
using OuputServices.Kafka.Entities;

namespace InputServices.Test;

internal class TestMocksProvider
{
    public static string GoodBootstrapServers => "192.168.0.127:9092";
    public string BadBootstrapServers => "192.168.0.127:9091";
    public IEnumerable<string> InputTopics { get; set; }
    public IEnumerable<string> OutputTopics { get; set; }

    public ClientConfig ClientConfig(string bootstrapServers) => new()
    {
        BootstrapServers = bootstrapServers,
        ClientId = Dns.GetHostName(),
        Acks = Acks.All
    };


    internal IProducer<int, string> GetStringProducer()
    {
        var producerConfig = new ProducerConfigFactory(ClientConfig(GoodBootstrapServers)).GetDefaultProducerConfig();
        var producerFactory = new ProducerFactory(producerConfig);
        return producerFactory.CreateStringProducer();
    }

    internal IConsumer<int, string> GetStringConsumer()
    {
        var consumerConfig = new ConsumerConfigFactory(ClientConfig(GoodBootstrapServers)).GetDefaultConsumerConfig();
        var producerFactory = new ConsumerFactory(consumerConfig);
        return producerFactory.CreateStringConsumer();
    }

    void f()
    {
        // to setup
        var kafkaInputConfig = new KafkaInputConfig() { Client = ClientConfig(GoodBootstrapServers), Topics = InputTopics };
        var kafkaInputService = new KafkaInputService(kafkaInputConfig);
    }
}
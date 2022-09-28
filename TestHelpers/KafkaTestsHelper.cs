using System.Net;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using KafkaInteractor;
using OuputServices.Kafka.Entities;

namespace InputServices.Test;

public class KafkaTestsHelper
{
    public string GoodBootstrapServers { get; set; }
    public string BadBootstrapServers { get; set; }

    public IEnumerable<string> InputTopics { get; set; }
    public IEnumerable<string> OutputTopics { get; set; }

    public KafkaTestsHelper()
    {
        //todo fix dat
        InputTopics = new List<string>() { "requestTopic" };
        OutputTopics = new List<string>() { "responseTopic" };
        GoodBootstrapServers = "192.168.0.127:9092";
        BadBootstrapServers = "192.168.0.127:9091";
    }

    public ClientConfig GetClientConfig(string bootstrapServers) => new()
    {
        BootstrapServers = bootstrapServers,
        ClientId = Dns.GetHostName(),
        Acks = Acks.All
    };

    public ClientConfig GetClientConfig() => new()
    {
        BootstrapServers = GoodBootstrapServers,
        ClientId = Dns.GetHostName(),
        Acks = Acks.All
    };

    public async Task CreateTopicAsync(string topicName, CancellationToken token)
    {
        var adminClient = GetAdminClient();
        try
        {
            while (!token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
                var topicSpecification = new TopicSpecification() { Name = topicName, NumPartitions = 4 };
                var topicSpecifications = new List<TopicSpecification>() { topicSpecification };
                await adminClient.CreateTopicsAsync(topicSpecifications,
                    new CreateTopicsOptions() { OperationTimeout = TimeSpan.FromSeconds(10), RequestTimeout = TimeSpan.FromSeconds(10), });
            }
        }
        catch (CreateTopicsException e)
        {
            //ignored
        }
    }

    public async Task DeleteTopicAsync(string topicName, CancellationToken token)
    {
        var adminClient = GetAdminClient();
        try
        {
            while (!token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
                var topics = new[] { topicName };
                await adminClient.DeleteTopicsAsync(topics,
                    new DeleteTopicsOptions() { OperationTimeout = TimeSpan.FromSeconds(10), RequestTimeout = TimeSpan.FromSeconds(10), });
            }
        }
        catch (DeleteTopicsException)
        {
            //ignored
        }
    }

    public IProducer<int, string> GetStringProducer()
    {
        var producerConfig = new ProducerConfigFactory(GetClientConfig(GoodBootstrapServers)).GetDefaultProducerConfig();
        var producerFactory = new ProducerFactory(producerConfig);
        return producerFactory.CreateStringProducer();
    }

    public IConsumer<int, string> GetStringConsumer()
    {
        var consumerConfig = new ConsumerConfigFactory(GetClientConfig(GoodBootstrapServers)).GetDefaultConsumerConfig();
        var producerFactory = new ConsumerFactory(consumerConfig);
        return producerFactory.CreateStringConsumer();
    }

    public IAdminClient GetAdminClient()
    {
        return new AdminClientBuilder(GetClientConfig(GoodBootstrapServers)).Build();
    }

    public bool IsServerAvailable()
    {
        var topics = new List<string>() { "__consumer_offsets" };
        try
        {
            foreach (var topic in topics)
            {
                var adminClient = GetAdminClient();
                adminClient.GetMetadata(topic, TimeSpan.FromSeconds(4));
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
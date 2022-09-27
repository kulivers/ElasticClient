using Confluent.Kafka;
using IOServices.Api;
using KafkaInteractor;
using Localization;

namespace InputServices;

public class KafkaInputService : IInputService, IDisposable
{
    private static readonly string TopicNotAvailable = IOServicesRecources.TopicNotAvailable;

    private readonly IEnumerable<string> _inputTopics;
    private readonly ConsumerConfig _consumerConfig;
    private IConsumer<int, string> StringConsumer { get; }

    public KafkaInputService(KafkaInputConfig config) : this(new ConsumerConfigFactory(config.Client).GetDefaultConsumerConfig(), config.Topics)
    {
    }

    public KafkaInputService(ConsumerConfig consumerConfig, IEnumerable<string> inputTopicses)
    {
        _inputTopics = inputTopicses;
        _consumerConfig = consumerConfig;
        var consumerFactory = new ConsumerFactory(_consumerConfig);
        StringConsumer = consumerFactory.CreateStringConsumer();
    }

    private int MessagesToCommit = 1000;
    public event EventHandler<object>? OnReceive;

    public async Task StartReceive(CancellationToken token)
    {
        var messagesToCommit = MessagesToCommit;
        StringConsumer.Subscribe(_inputTopics);

        while (!token.IsCancellationRequested)
        {
            var receive = StringConsumer.Consume(token);
            if (receive == null)
            {
                continue;
            }

            var value = receive.Message.Value;
            CallOnMessageEvent(value);

            messagesToCommit--;
            if (messagesToCommit <= 0)
            {
                StringConsumer.Commit();
                messagesToCommit = MessagesToCommit;
            }
        }
    }

    public void CheckHealth()
    {
        var adminConfig = new AdminClientConfig(_consumerConfig);
        var adminClient = new AdminClientBuilder(adminConfig).Build();
        using (adminClient)
        {
            foreach (var topic in _inputTopics)
            {
                var metadata = adminClient.GetMetadata(topic, new TimeSpan(0, 0, 10));
                foreach (var topicMetadata in metadata.Topics)
                {
                    if (topicMetadata.Error.IsError)
                    {
                        throw new IOException(string.Format(TopicNotAvailable, topicMetadata.Topic, topicMetadata.Error.Reason));
                    }
                }
            }
        }
    }

    private protected virtual void CallOnMessageEvent(string e)
    {
        OnReceive?.Invoke(this, e);
    }

    public void Dispose()
    {
        StringConsumer.Dispose();
    }
}
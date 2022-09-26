using Confluent.Kafka;
using IOServices.Api;
using KafkaInteractor;

namespace InputServices;

public class KafkaInputService : IInputService
{
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
        StringConsumer = new ConsumerFactory(_consumerConfig).CreateStringConsumer();
    }


    public event EventHandler<object>? OnReceive;

    public async Task StartReceive(CancellationToken token)
    {
        using (StringConsumer)
        {
            StringConsumer.Subscribe(_inputTopics);

            while (!token.IsCancellationRequested)
            {
                var receive = StringConsumer.Consume(token);
                if (receive == null) continue;
                var value = receive.Message.Value;
                CallOnMessageEvent(value);
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
                foreach (var t in metadata.Topics)
                {
                    if (t.Error.IsError)
                    {
                        throw new IOException($"topic {t.Topic} is not available. Reason: {t.Error.Reason}");
                    }
                }
            }
        }
    }

    private protected virtual void CallOnMessageEvent(string e)
    {
        OnReceive?.Invoke(this, e);
    }
}
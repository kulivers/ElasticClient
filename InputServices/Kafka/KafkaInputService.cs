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

    public KafkaInputService(ConsumerConfig consumerConfig, IEnumerable<string> inputTopics)
    {
        _inputTopics = inputTopics;
        _consumerConfig = consumerConfig;
        var consumerFactory = new ConsumerFactory(_consumerConfig);
        StringConsumer = consumerFactory.CreateStringConsumer();
    }

    private const int MessagesToCommit = 1000;
    public event EventHandler<InputResponseModel>? OnReceive;
    public bool ReceivedAny { get; private set; }

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

    public void CheckHealth(double secondsToResponse)
    {
        var adminConfig = new AdminClientConfig(_consumerConfig);
        var adminClient = new AdminClientBuilder(adminConfig).Build();
        using (adminClient)
        {
            foreach (var topic in _inputTopics)
            {
                var metadata = adminClient.GetMetadata(topic,  TimeSpan.FromSeconds(secondsToResponse));
                foreach (var topicMetadata in metadata.Topics)
                {
                    if (topicMetadata.Error.IsError)
                    {
                        var exMessage = string.Format(TopicNotAvailable, topicMetadata.Topic, topicMetadata.Error.Reason);
                        throw new IOException(exMessage);
                    }
                }
            }
        }
    }

    private protected virtual void CallOnMessageEvent(string e)
    {
        ReceivedAny = true;
        var inputServiceResponseModel = new InputResponseModel(e);
        OnReceive?.Invoke(this, inputServiceResponseModel);
    }

    public void Dispose()
    {
        StringConsumer.Dispose();
    }
}
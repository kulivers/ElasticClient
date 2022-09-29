using Confluent.Kafka;
using IOServices.Api;
using KafkaInteractor;
using Localization;

namespace InputServices;

public class KafkaInput : IInputService, IDisposable
{
    private static readonly string TopicNotAvailable = IOServicesRecources.TopicNotAvailable;
    private readonly IEnumerable<string> _inputTopics;
    private readonly ConsumerConfig _consumerConfig;
    private IConsumer<int, string> StringConsumer { get; }

    public KafkaInput(KafkaInputConfig config) : this(new ConsumerConfigFactory(config.Client).GetDefaultConsumerConfig(), config.Topics)
    {
    }

    public KafkaInput(ConsumerConfig consumerConfig, IEnumerable<string> inputTopics)
    {
        _inputTopics = inputTopics;
        _consumerConfig = consumerConfig;
        var consumerFactory = new ConsumerFactory(_consumerConfig);
        StringConsumer = consumerFactory.CreateStringConsumer();
    }

    private const int MessagesToCommit = 1000;
    public event EventHandler<InputModel>? OnReceive;


    public async Task StartReceive(CancellationToken token)
    {
        var messagesToCommit = MessagesToCommit;
        StringConsumer.Subscribe(_inputTopics);

        while (!token.IsCancellationRequested)
        {
            var received = StringConsumer.Consume(token);
            if (received == null)
            {
                continue;
            }


            CallOnMessageEvent(received);

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
                var metadata = adminClient.GetMetadata(topic, TimeSpan.FromSeconds(secondsToResponse));
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

    private protected virtual void CallOnMessageEvent(ConsumeResult<int, string> recieved)
    {
        var message = recieved.Message.Value;
        var inputModel = new InputModel(message);
        OnReceive?.Invoke(this, inputModel);
    }

    public void Dispose()
    {
        StringConsumer.Dispose();
    }
}
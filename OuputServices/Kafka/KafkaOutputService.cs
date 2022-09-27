using Confluent.Kafka;
using InputServices;
using IOServices.Api;
using KafkaInteractor;
using Localization;

namespace OuputServices;

public class KafkaOutputService : IOutputService, IDisposable
{
    private static readonly string TopicNotAvailableText = IOServicesRecources.TopicNotAvailable;
    private static readonly string CantSendMessage = IOServicesRecources.CantSendMessageOfType;

    private readonly ProducerConfig _producerConfig;

    private IProducer<int, string> StringProducer { get; }

    private IEnumerable<string> OutputTopics { get; }

    public event EventHandler<object>? OnSend;

    public KafkaOutputService(KafkaOutputConfig kafkaOutputConfig) : this(new ProducerConfig(kafkaOutputConfig.Client), kafkaOutputConfig.Topics)
    {
    }

    public KafkaOutputService(ProducerConfig producerConfig, IEnumerable<string> outputTopics)
    {
        _producerConfig = producerConfig;
        StringProducer = new ProducerFactory(_producerConfig).CreateStringProvider();
        OutputTopics = outputTopics;
    }

    private async Task<object> SendString(string o, CancellationToken token)
    {
        var message = new Message<int, string>() { Value = o };
        var deliveryResults = new List<DeliveryResult<int, string>>();
        foreach (var topic in OutputTopics)
        {
            var deliveryResult = await StringProducer.ProduceAsync(topic, message, token);
            deliveryResults.Add(deliveryResult);
            CallOnSendEvent(deliveryResult);
        }
        return deliveryResults;
    }

    public async Task<object> Send(object o, CancellationToken token)
    {
        if (o is string message)
        {
            return await SendString(message, token);
        }

        throw new NotImplementedException(string.Format(CantSendMessage, o.GetType()));
    }

    private void CallOnSendEvent(object deliveryResult)
    {
        OnSend?.Invoke(this, deliveryResult);
    }


    public void CheckHealth()
    {
        var adminConfig = new AdminClientConfig(_producerConfig);
        var adminClient = new AdminClientBuilder(adminConfig).Build();
        using (adminClient)
        {
            foreach (var outputTopic in OutputTopics)
            {
                var metadata = adminClient.GetMetadata(outputTopic, new TimeSpan(0, 0, 10));
                foreach (var topic in metadata.Topics)
                {
                    if (topic.Error.IsError)
                    {
                        throw new IOException(string.Format(TopicNotAvailableText, topic.Topic, topic.Error.Reason));
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        StringProducer.Dispose();
    }
}
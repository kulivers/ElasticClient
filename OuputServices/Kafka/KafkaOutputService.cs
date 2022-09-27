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
        StringProducer = new ProducerFactory(_producerConfig).CreateStringProducer();
        OutputTopics = outputTopics;
    }

    private async Task<OutputResponseModel> SendString(string toSend, CancellationToken token)
    {
        var message = new Message<int, string>() { Value = toSend };
        var outputModels = new List<OutputResponseModel>();

        foreach (var topic in OutputTopics)
        {
            try
            {
                var deliveryResult = await StringProducer.ProduceAsync(topic, message, token);
                CallOnSendEvent(deliveryResult);
                var outputModel = new OutputResponseModel(deliveryResult);
                outputModels.Add(outputModel);
            }
            catch (Exception e)
            {
                var outputModel = new OutputResponseModel(e);
                outputModels.Add(outputModel);
            }
        }

        return new OutputResponseModel(outputModels);
    }

    public async Task<OutputResponseModel> Send(object toSend, CancellationToken token)
    {
        if (toSend is string message)
        {
            return await SendString(message, token);
        }

        throw new NotImplementedException(string.Format(CantSendMessage, toSend.GetType()));
    }

    private void CallOnSendEvent(object deliveryResult)
    {
        OnSend?.Invoke(this, deliveryResult);
    }


    public void CheckHealth(double secondsToResponse)
    {
        var adminConfig = new AdminClientConfig(_producerConfig);
        var adminClient = new AdminClientBuilder(adminConfig).Build();
        using (adminClient)
        {
            foreach (var outputTopic in OutputTopics)
            {
                var metadata = adminClient.GetMetadata(outputTopic,  TimeSpan.FromSeconds(secondsToResponse));
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
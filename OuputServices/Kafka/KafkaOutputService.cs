using Confluent.Kafka;
using InputServices;
using IOServices.Api;
using KafkaInteractor;

namespace OuputServices;

public class KafkaOutputService : IOutputService
{
    private readonly ProducerConfig _producerConfig;
    private IProducer<int, string> StringProducer { get; }
    private IEnumerable<string> OutputTopics { get; }

    public IOutputService Create(string path)
    {
        var config = KafkaOutputConfig.FromYaml(path);
        return new KafkaOutputService(config);
    }

    public event EventHandler<object>? OnSend;

    public async Task Send(string o, CancellationToken token)
    {
        var message = new Message<int, string>() { Value = o };
        using (StringProducer)
        {
            foreach (var topic in OutputTopics)
            {
                var deliveryResult = await StringProducer.ProduceAsync(topic, message, token);
                CallOnSendEvent(deliveryResult);
            }
        }
    }

    public async Task Send(object o, CancellationToken token)
    {
        if (o is string message)
        {
            await Send(message, token);
            return;
        }

        throw new NotImplementedException("Cant send not string message");
    }

    private void CallOnSendEvent(object deliveryResult)
    {
        OnSend?.Invoke(this, deliveryResult);
    }

    public KafkaOutputService(KafkaOutputConfig kafkaOutputConfig) : this(new ProducerConfig(kafkaOutputConfig.Client), kafkaOutputConfig.Topics)
    {
    }

    public KafkaOutputService(ProducerConfig producerConfig, IEnumerable<string> outputTopics)
    {
        _producerConfig = producerConfig;
        StringProducer = new ProducerFactory(_producerConfig).CreateStringProvider();
        OutputTopics = outputTopics;
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
                        throw new IOException($"topic {topic.Topic} is not available. Reason: {topic.Error.Reason}");
                    }
                }
            }
        }
    }
}
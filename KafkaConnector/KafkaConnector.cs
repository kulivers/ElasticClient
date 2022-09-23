using System.Runtime.CompilerServices;
using Confluent.Kafka;
using KafkaInteractor;

namespace ProcessorsRunner;

public class KafkaConnector : IConnector
{
    public IoType Type => IoType.Kafka;
    public string DestinationService { get; }
    private string ConfigPath { get; }
    private string InputTopic { get; }
    private string? OutputTopic { get; }
    private IConsumer<int, string> StringConsumer { get; }
    private IProducer<int, string> StringProducer { get; }

    public event EventHandler<string>? OnReceive;

    public KafkaConnector(ConnectorConfig connectorsConfig)
    {
        DestinationService = connectorsConfig.Destination;
        ConfigPath = connectorsConfig.Properties["config"];
        InputTopic = connectorsConfig.Properties["inputTopic"];
        OutputTopic = connectorsConfig.Properties.ContainsKey("outputTopic")
            ? connectorsConfig.Properties["outputTopic"]
            : null;
        StringConsumer = new ConsumerFactory(ConfigPath).CreateStringConsumer();
        StringProducer = new ProducerFactory(ConfigPath).CreateStringProvider();
        CheckHealth();
    }

    public async Task StartReceive(CancellationToken token)
    {
        StringConsumer.Subscribe(InputTopic);
        while (!token.IsCancellationRequested)
        {
            var receive = StringConsumer.Consume(token);
            if (receive == null) continue;
            var value = receive.Message.Value;
            CallOnMessageEvent(value);
        }
    }

    public void CheckHealth()
    {
        var producerConfig = new ProducerFactory(ConfigPath).ProducerConfig;
        var adminConfig = new AdminClientConfig(producerConfig);
        var adminClient = new AdminClientBuilder(adminConfig).Build();
        var metadata = adminClient.GetMetadata(InputTopic, new TimeSpan(0,0,10));
        
        foreach (var topic in metadata.Topics)
        {
            if (topic.Error.IsError)
            {
                throw new IOException($"topic {topic.Topic} is not available. Reason: {topic.Error.Reason}");
            }
        }
    }

    private protected virtual void CallOnMessageEvent(string e)
    {
        OnReceive?.Invoke(this, e);
    }
}
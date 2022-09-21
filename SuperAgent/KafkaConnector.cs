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

    public event EventHandler<string> OnReceive;

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
        
    }

    public async Task<DeliveryResult<int, string>?> Send(string message, CancellationToken token = default)
    {
        if (OutputTopic == null)
            return null;
        var kafkaMessage = new Message<int, string>() { Value = message };
        return await StringProducer.ProduceAsync(OutputTopic, kafkaMessage, token);
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

    private protected virtual void CallOnMessageEvent(string e)
    {
        OnReceive?.Invoke(this, e);
        
    }
}
using Confluent.Kafka;

namespace ProcessorsRunner;

public interface IConnector
{
    IoType Type { get; }
    string DestinationService { get; }
    event EventHandler<string> OnReceive;
    Task StartReceive(CancellationToken token);
    void CheckHealth();
}
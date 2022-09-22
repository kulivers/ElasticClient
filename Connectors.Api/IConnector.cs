using Confluent.Kafka;

namespace ProcessorsRunner;

public interface IConnector
{
    IoType Type { get; }
    string DestinationService { get; }
    event EventHandler<string> OnReceive;
    Task<object?> Send(string message, CancellationToken token);
    Task StartReceive(CancellationToken token);
    void CheckAvailable();

    
}
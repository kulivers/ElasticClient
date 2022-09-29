using IOServices.Api;

namespace ProcessorsRunner;

public interface IConnector
{
    IInputService InputService { get; }
    IOutputService? OutputService { get; }
    string DestinationProcessor { get; }
    event EventHandler<InputModel> OnReceive;
    Task StartReceive(CancellationToken token);
    void CheckHealth();
    Task SendToOutputService(object toSend);
}
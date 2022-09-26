using IOServices.Api;

namespace ProcessorsRunner;

public class Connector : IConnector
{
    public IInputService InputService { get; }
    public IOutputService? OutputService { get; }
    public string DestinationProcessor { get; }
    public event EventHandler<object>? OnReceive;

    internal Connector(string destinationProcessor, IInputService inputService, IOutputService? outputService = null)
    {
        InputService = inputService;
        OutputService = outputService;
        DestinationProcessor = destinationProcessor;
        InputService.OnReceive += CallOnReceiveMethod;
    }

    private void CallOnReceiveMethod(object? sender, object e)
    {
        OnReceive?.Invoke(sender, e);
        var cts = new CancellationTokenSource(new TimeSpan(0, 0, 5));
        OutputService?.Send(e, cts.Token).Start();
    }

    public async Task StartReceive(CancellationToken token)
    {
        InputService.StartReceive(token).Start(); //todo check that one
    }

    public void CheckHealth()
    {
        InputService.CheckHealth();
        OutputService?.CheckHealth();
    }
}
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

    private async Task SendToOutputService(object e)
    {
        var cts = new CancellationTokenSource(new TimeSpan(10, 10, 5));//todo after all
        if (OutputService != null)
        {
            var result = await OutputService.Send(e, cts.Token); // todo if canceled - catch that
        }
    }
    private async void CallOnReceiveMethod(object? sender, object e)
    {
        OnReceive?.Invoke(sender, e);
        await SendToOutputService(e);
    }

    public async Task StartReceive(CancellationToken token)
    {
        InputService.StartReceive(token).Start(); 
    }

    public void CheckHealth()
    {
        InputService.CheckHealth();
        OutputService?.CheckHealth();
    }
}
using IOServices.Api;

namespace ProcessorsRunner;

public class Connector : IConnector
{
    public IInputService InputService { get; }
    public IOutputService? OutputService { get; }
    public string DestinationProcessor { get; }
    private const double SecondsToResponse = 5;
    public event EventHandler<InputModel>? OnReceive;

    internal Connector(string destinationProcessor, IInputService inputService, IOutputService? outputService = null)
    {
        InputService = inputService;
        OutputService = outputService;
        DestinationProcessor = destinationProcessor;
        InputService.OnReceive += CallOnReceive;
    }

    private async void CallOnReceive(object? sender, object inputResponseModel)
    {
        var casted = (InputModel)inputResponseModel;
        OnReceive?.Invoke(sender, casted);
    }

    public async Task StartReceive(CancellationToken token)
    {
        InputService.StartReceive(token).Start();
    }

    public void CheckHealth()
    {
        InputService.CheckHealth(SecondsToResponse);
        OutputService?.CheckHealth(SecondsToResponse);
    }

    public async Task SendToOutputService(object toSend)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(SecondsToResponse)); 
        if (OutputService != null)
        {
            try
            {
                //here we can do something with response after output
                var result = await OutputService.Send(toSend, cts.Token);
            }
            catch (TaskCanceledException exception)
            {
                var outputModel = new OutputResponseModel(exception);
                var result = outputModel;
            }
            catch (Exception exception)
            {
                var outputModel = new OutputResponseModel(exception);
                var result = outputModel;
            }
        }
    }
}
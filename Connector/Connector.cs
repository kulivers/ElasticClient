using IOServices.Api;

namespace ProcessorsRunner;

public class Connector : IConnector
{
    public IInputService InputService { get; }
    public IOutputService? OutputService { get; }
    public string DestinationProcessor { get; }
    private double SecondsToResponse => 5;
    public event EventHandler<InputResponseModel>? OnReceive;

    internal Connector(string destinationProcessor, IInputService inputService, IOutputService? outputService = null)
    {
        InputService = inputService;
        OutputService = outputService;
        DestinationProcessor = destinationProcessor;
        InputService.OnReceive += CallOnReceiveMethod;
    }

    private async void CallOnReceiveMethod(object? sender, object inputResponseModel)
    {
        var casted = (InputResponseModel)inputResponseModel;
        OnReceive?.Invoke(sender, casted);
        if (casted.Data != null)
        {
            await SendToOutputService(casted.Data);
        }
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

    private async Task SendToOutputService(object toSend)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(SecondsToResponse)); 
        if (OutputService != null)
        {
            //here we can do something with response after output
            try
            {
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
    using System.Net.Http.Json;
using IOServices.Api;
using Newtonsoft.Json;

namespace ProcessorsRunner;

public class Connector : IConnector
{
    public IInputService InputService { get; }
    public IOutputService? OutputService { get; }
    public string DestinationProcessor { get; }
    private const double SecondsToResponse = 5;
    public event EventHandler<string>? OnReceive;

    internal Connector(string destinationProcessor, IInputService inputService, IOutputService? outputService = null)
    {
        InputService = inputService;
        OutputService = outputService;
        DestinationProcessor = destinationProcessor;
        InputService.OnReceive += CallOnReceive;
    }

    private void CallOnReceive(object? sender, object recieved)
    {
        OnReceive?.Invoke(sender, (string)recieved);
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
        Console.WriteLine(JsonConvert.SerializeObject(toSend));
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
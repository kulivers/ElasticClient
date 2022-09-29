namespace IOServices.Api;

public interface IInputService
{
    event EventHandler<object> OnReceive;
    Task StartReceive(CancellationToken token);
    void CheckHealth(double secondsToResponse);
}
namespace IOServices.Api;

public interface IInputService
{
    event EventHandler<InputModel> OnReceive;
    Task StartReceive(CancellationToken token);
    void CheckHealth(double secondsToResponse);
}
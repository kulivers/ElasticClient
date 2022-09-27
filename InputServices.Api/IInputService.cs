namespace IOServices.Api;

public interface IInputService
{
    event EventHandler<InputResponseModel> OnReceive;
    Task StartReceive(CancellationToken token);
    void CheckHealth(double secondsToResponse);
}
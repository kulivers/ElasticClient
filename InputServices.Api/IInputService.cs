namespace IOServices.Api;

public interface IInputService
{
    event EventHandler<InputResponseModel> OnReceive;
    Task StartReceive(CancellationToken appStop);
    void CheckHealth(double secondsToResponse);
}
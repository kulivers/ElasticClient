namespace IOServices.Api;

public interface IOutputService
{
    event EventHandler<object> OnSend;
    Task Send(object o, CancellationToken token);
    void CheckHealth();
}
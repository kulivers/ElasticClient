namespace IOServices.Api;

public interface IOutputService
{
    event EventHandler<object> OnSend;
    Task<object> Send(object o, CancellationToken token);
    void CheckHealth();
}
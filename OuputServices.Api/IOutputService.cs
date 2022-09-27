namespace IOServices.Api;

public interface IOutputService
{
    event EventHandler<object> OnSend;
    Task<OutputResponseModel> Send(object toSend, CancellationToken token);
    void CheckHealth(double seconds);
}
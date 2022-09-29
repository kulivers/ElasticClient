namespace IOServices.Api;

public class InputModel
{
    public bool Success { get; set; }
    public Exception? Exception { get; set; }
    public string? Data { get; set; }


    public InputModel(string? data = null)
    {
        Success = true;
        Exception = null;
        Data = data;
    }
    
    public InputModel(Exception? exception)
    {
        Success = false;
        Exception = exception;
        Data = null;
    }

    public InputModel(bool success, Exception? exception, string? data)
    {
        Success = success;
        Exception = exception;
        Data = data;
    }


}
namespace IOServices.Api;

public class InputResponseModel
{
    public bool Success { get; set; }
    public Exception? Exception { get; set; }
    public string? Data { get; set; }


    public InputResponseModel(string? data = null)
    {
        Success = true;
        Exception = null;
        Data = data;
    }
    
    public InputResponseModel(Exception? exception)
    {
        Success = false;
        Exception = exception;
        Data = null;
    }

    public InputResponseModel(bool success, Exception? exception, string? data)
    {
        Success = success;
        Exception = exception;
        Data = data;
    }


}
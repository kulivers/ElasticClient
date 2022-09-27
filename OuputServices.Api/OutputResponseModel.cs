namespace IOServices.Api;

public class OutputResponseModel
{
    public bool Success { get; set; }
    public Exception? Exception { get; set; }
    public object? Data { get; set; }

    public IEnumerable<OutputResponseModel>? OutputModels { get; set; }

    public OutputResponseModel(IEnumerable<OutputResponseModel> outputModels)
    {
        var enumerable = outputModels as OutputResponseModel[] ?? outputModels.ToArray();
        Data = null;
        Exception = null;
        if (enumerable.All(model => model.Success))
        {
            Success = true;
        }
        OutputModels = enumerable;
    }
    public OutputResponseModel(object? data = null)
    {
        Success = true;
        Exception = null;
        Data = data;
    }
    
    public OutputResponseModel(Exception? exception)
    {
        Success = false;
        Exception = exception;
        Data = null;
    }

    public OutputResponseModel(bool success, Exception? exception, object? data)
    {
        Success = success;
        Exception = exception;
        Data = data;
    }
    
}
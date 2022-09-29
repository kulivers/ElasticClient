namespace Processor.Api;

public class ProcessorOutput<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public Exception? Exception { get; set; }

    public ProcessorOutput(T? data)
    {
        Success = true;
        Data = data;
        Exception = null;
    }

    public ProcessorOutput(Exception exception)
    {
        Success = false;
        Exception = exception;
    }
}
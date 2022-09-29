namespace Processor.Api;

public class ProcessorOutput<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public Exception? Exception { get; set; }
}
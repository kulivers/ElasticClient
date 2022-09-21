using Processor;

namespace ProcessorsRunner;

public interface IProcessorsContainer : IEnumerable<IProcessor>
{
    List<IProcessor?> Processors { get; }
    IProcessor<TIn, TOut>? GetService<TIn, TOut>();
    void AddService(IProcessor? processor);
    public IProcessor? GetService(string serviceName);

    public TOut? Process<TIn, TOut>(string serviceName, TIn input);
    public object? Process(string processorName, string message);
}
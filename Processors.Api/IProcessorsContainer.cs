using Processor;

namespace ProcessorsRunner;

public interface IProcessorsContainer : IEnumerable<IProcessor>
{
    List<IProcessor?> Processors { get; }
    IProcessor<TIn, TOut>? GetProcessor<TIn, TOut>();
    void AddProcessor(IProcessor? processor);
    public IProcessor? GetProcessor(string serviceName);

    public TOut? Process<TIn, TOut>(string serviceName, TIn input, CancellationToken token);
    public object? Process(string processorName, string? message, CancellationToken token);
}
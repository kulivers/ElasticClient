using Processor;

namespace ProcessorsRunner;

public interface IProcessorsContainer : IEnumerable<IProcessor>
{
    List<IProcessor?> Processors { get; }
    IProcessor<TIn, TOut>? GetService<TIn, TOut>();
    void AddService(IProcessor? processor);
    public IProcessor? GetService(string serviceName);

}
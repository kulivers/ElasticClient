namespace Processor;

public interface IProcessor  
{
    string ServiceName { get; }
    ProcessorConfig ProcessorConfig { get; }
    void CheckHealth();
}

public interface IProcessor<TIn, TOut> : IProcessor
{
    public TOut Process(TIn value, CancellationToken token);
    public Task<TOut> ProcessAsync(TIn value, CancellationToken token);
}

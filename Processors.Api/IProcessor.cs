namespace Processor;

public interface IProcessor  
{
    string ServiceName { get; }
    ProcessorConfig ProcessorConfig { get; }
    Task CheckHealth();

}

public interface IProcessor<TIn, TOut> : IProcessor
{
    public TOut Process(TIn value);
    public Task<TOut> ProcessAsync(TIn value);
}


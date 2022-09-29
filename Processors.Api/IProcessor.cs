using Processor.Api;

namespace Processor;

public interface IProcessor  
{
    string Name { get; }
    ProcessorConfig ProcessorConfig { get; }
    void CheckHealth();
}

public interface IProcessor<TIn, TOut> : IProcessor
{
    public ProcessorOutput<TOut> Process(TIn value, CancellationToken token);
    public Task<ProcessorOutput<TOut>> ProcessAsync(TIn value, CancellationToken token);
}

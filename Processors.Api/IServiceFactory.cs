namespace Processor;

public interface IProcessorFactory<TIn, TOut>
{
    IProcessor GetOrCreateProcessor(ProcessorConfig config);
}
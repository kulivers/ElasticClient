namespace Processor;

public interface IProcessorFactory<TIn, TOut>
{
    IProcessor GetOrCreateProcessor(ServiceConfig config);
}
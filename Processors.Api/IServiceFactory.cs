namespace Processor;

public interface IProcessorFactory<TIn, TOut>
{
    IProcessor<TIn, TOut> GetOrCreateService(ServiceConfig config);
}
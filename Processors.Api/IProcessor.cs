namespace Processor;

public interface IProcessor  
{
    string ServiceName { get; }
    ServiceConfig ServiceConfig { get; }
    int GetHashCode();

}

public interface IProcessor<TIn, TOut> : IProcessor
{
    public TOut Process(TIn value);
}


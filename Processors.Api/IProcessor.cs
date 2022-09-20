namespace Processor;

public interface IProcessor 
{
    string ServiceName { get; }
}

public interface IProcessor<TIn, TOut> : IProcessor
{
    public TOut Process(TIn value);
}


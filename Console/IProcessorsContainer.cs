using Processor;

namespace ProcessorsRunner;

public interface IProcessorsContainer : IEnumerable<IProcessor>
{
    HashSet<IProcessor> Processors { get; }
}
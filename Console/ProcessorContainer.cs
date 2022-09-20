using System.Collections;
using System.Reflection;
using ElasticClient;
using Processor;
using ProcessorsRunner;

internal class ProcessorContainer : IProcessorsContainer
{
    public List<IProcessor?> Processors { get; }

    public ProcessorContainer(ServicesConfig servicesConfig)
    {
        Processors = new List<IProcessor?>();
        foreach (var config in servicesConfig.Services)
        {
            var assembly = Assembly.LoadFrom(config.Dll);
            var assTypes = assembly.GetTypes();
            var factories = FindFactories(assTypes);
            foreach (var factoryType in factories)
            {
                var factoryInstance = Activator.CreateInstance(factoryType);
                var factoryMethodName = typeof(IProcessorFactory<,>).GetMethods().First().Name;
                var factoryMethod = factoryType.GetMethod(factoryMethodName);
                var serviceInstance = factoryMethod?.Invoke(factoryInstance, new[] { config });
                if (serviceInstance is IProcessor processor)
                    AddService(processor);
            }
        }
    }

    private IEnumerable<Type> FindFactories(IEnumerable<Type> types)
    {
        var typesWithAttribute = types.Where(it =>
            it.GetCustomAttributes().Any(attribute => attribute.GetType() == typeof(ProcessElementAttribute)));
        foreach (var type in typesWithAttribute)
        {
            foreach (var attribute in type.GetCustomAttributes())
            {
                if (attribute is ProcessElementAttribute { Type: ProcessingAttributeBehaviourType.Factory })
                    yield return type;
            }
        }
    }

    public void AddService(IProcessor? processor) => Processors.Add(processor);

    public IProcessor<TIn, TOut>? GetService<TIn, TOut>()
    {
        IProcessor<TIn, TOut>? toReturn = null;
        foreach (var processor in Processors)
        {
            if (processor is IProcessor<TIn, TOut> other)
                toReturn = other;
        }

        return toReturn;
    }

    public IProcessor? GetService(string serviceName) => Processors.FirstOrDefault(p => p?.ServiceName == serviceName);
    public IEnumerator<IProcessor> GetEnumerator() => Processors.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Processors).GetEnumerator();
}
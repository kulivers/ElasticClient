using System.Collections;
using System.Reflection;
using ElasticClient;
using Processor;
using ProcessorsRunner;

internal class ProcessorContainer : IProcessorsContainer
{
    public HashSet<IProcessor> Processors { get; }
    

    public ProcessorContainer(ServicesConfig servicesConfig)
    {
        Processors = new HashSet<IProcessor>();
        foreach (var config in servicesConfig.Services)
        {
            var assembly = Assembly.LoadFrom(config.Dll);
            var fabrics = FindFactories(assembly.GetTypes());
            foreach (var factoryType in fabrics)
            {

                // var factoryInterface = fabricType.GetInterfaces()
                //     .FirstOrDefault(type => type.Name == "IProcessorFactory`2");
                // var genericArgumentTypes = factoryInterface.GenericTypeArguments;
                // Type inType = genericArgumentTypes.ElementAt(0);
                // Type outType = genericArgumentTypes.ElementAt(1);
                // Type serviceType = fabricMethod.ReturnParameter.ParameterType; //returnType
                
                var factoryInstance = Activator.CreateInstance(factoryType);
                var factoryMethod = factoryType.GetMethod("GetOrCreateService");
                var serviceInstance = factoryMethod?.Invoke(factoryInstance, new[] { config });
                if (serviceInstance is IProcessor processor)
                {
                    Add(processor);
                }
            }
        }
    }

    public IEnumerable<Type> FindFactories(IEnumerable<Type> types)
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

    public void Add(IProcessor processor) => Processors.Add(processor);

    public IEnumerator<IProcessor> GetEnumerator()
    {
        return Processors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Processors).GetEnumerator();
    }
}
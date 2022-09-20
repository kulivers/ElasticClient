using System.Reflection;
using Newtonsoft.Json;
using Processor;
using ProcessorsRunner;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


public class Worker
{
    public IProcessorsContainer ProcessorsContainer { get; }

    public Worker(ServicesConfig servicesConfig)
    {
        ProcessorsContainer = new ProcessorContainer(servicesConfig);
    }

    public object? Process(string processorName, string message)
    {
        var service = ProcessorsContainer.GetService(processorName);
        
    }
}

class Program
{
    public static ServicesConfig Deserialize(string path)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) //todo egor mb it is not camel case
            .Build();
        var fileContent = File.ReadAllText(path);
        return deserializer.Deserialize<ServicesConfig>(fileContent);
    }

    public static (Type tIn, Type tOut) GetInpuOutputTypes(Type containterType)
    {
        var iProcessorInterface = containterType.FindInterfaces(MyInterfaceFilter, typeof(IProcessor<,>).Name).First();
        var genericArguments = iProcessorInterface.GetGenericArguments();
        var tIn = genericArguments.First();
        var tOut = genericArguments.Last();
        return (tIn, tOut);
    }

    public static object? MakeRequest(Type processorType, object processor, string jsonMessage)
    {
        var (tIn, tOut) = GetInpuOutputTypes(processorType);
        var esRequest = JsonConvert.DeserializeObject(jsonMessage, tIn);

        var method = processorType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .First(mi => mi.ReturnType == tOut && mi.GetParameters().Any(p => p.ParameterType == tIn) &&
                         mi.Name == "Process");
        var result = method.Invoke(processor, new[] { esRequest });

        return result;
    }

    public static void Main(string[] args)
    {
        var dllpathh = @"D:\Work\ElasticClient\ElasticProcessor\bin\Debug\net6.0\ElasticProcessor.dll";
        var cfgPath = @"D:\Work\ElasticClient\Console\config\worker.yaml";
        var serviceName = "Elasticsearch writer1";

        var servicesConfig = Deserialize(cfgPath);
        var container = new ProcessorContainer(servicesConfig);

        var processor = container.GetService(serviceName); //todo its example

        var result = MakeRequest(processor.GetType(), processor, GetMockEsRequestJson());
        var json = GetMockEsRequestJson();

        //process(processorname, message) пока что в контейнере
        // json->EsRequest=>Service
        //json -> условно знаем что в service1 -> узнаем из dll, что Tin = EsRequest -> newtownJson to EsRequest -> process  
    }

    public static string GetMockEsRequestJson() =>
        "{\"HostConfig\":{\"Scheme\":\"https\",\"Host\":\"localhost\",\"Port\":9200},\"RequestParameters\":" +
        "{\"Index\":\"test3\",\"Type\":\"_doc\",\"DocId\":null}," +
        "\"Data\":\"{\\\"name\\\": 312}\"}";

    public static bool MyInterfaceFilter(Type typeObj, object? criteriaObj) =>
        typeObj.ToString().Contains(criteriaObj.ToString());
}
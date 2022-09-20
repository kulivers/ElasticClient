using System.Reflection;
using ElasticClient;
using Processor;
using ProcessorsRunner;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


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

    public static void Main(string[] args)
    {
        var dllpathh = @"D:\Work\ElasticClient\ElasticProcessor\bin\Debug\net6.0\ElasticProcessor.dll";
        var cfgPath = @"D:\Work\ElasticClient\Console\config\worker.yaml";
        
        var servicesConfig = Deserialize(cfgPath);
        IProcessorsContainer processorsContainer = new ProcessorContainer(servicesConfig);
        
        
        // var serviceConfig = new ServiceConfig()
        // {
        //     Path = "D:\\Work\\ElasticClient\\Console\\elasticConfig.yaml", 
        //     ConfigType = ConfigType.Yaml,
        //     ServiceName = "ElasticProcessor"
        // };
        //
        // var processor = new ElasticProcessorFactory().GetOrCreateService(serviceConfig);
        // EsRequest request = GetMock();
        // EsResponse esResponse = processor.Process(request);
        // Console.WriteLine(esResponse.Success);
        // Console.WriteLine(esResponse.StatusCode);
        // Console.WriteLine(esResponse.Data);
    }
}
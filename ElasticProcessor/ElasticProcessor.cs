using Processor;

namespace ElasticClient;

[ProcessElement(nameof(ElasticProcessor), ProcessingAttributeBehaviourType.Processor)]
public class ElasticProcessor : IProcessor<EsRequest, EsResponse>
{
    private readonly EsClient _esClient;

    public ElasticProcessor(ServiceConfig config)
    {
        if (config.ConfigType != ConfigType.Yaml)
            throw new NotImplementedException();
        var clientConfig = EsClientConfig.GetFromFile(config.Config);
        _esClient = new EsClient(clientConfig);
    }
    
    public EsResponse Process(EsRequest value) => _esClient.WriteRecord(value);

    public string ServiceName => nameof(ElasticProcessor);
}
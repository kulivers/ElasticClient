using Processor;

namespace ElasticClient;

[ProcessElement(nameof(ElasticProcessor), ProcessingAttributeBehaviourType.Processor)]
public class ElasticProcessor : IProcessor<EsRequest, EsResponse>
{
    private readonly EsClient _esClient;
    public string ServiceName => ServiceConfig.ServiceName;
    public ServiceConfig ServiceConfig { get; }

    public ElasticProcessor(ServiceConfig config)
    {
        if (config.ConfigType != ConfigType.Yaml)
            throw new NotImplementedException();
        ServiceConfig = config;
        var clientConfig = EsClientConfig.GetFromFile(config.Config);
        _esClient = new EsClient(clientConfig);
    }

    public EsResponse Process(EsRequest value) => _esClient.WriteRecord(value);

    public TOut Process<TIn, TOut>(TIn value)
    {
        if (value is EsRequest esRequest)
        {
            var response = Process(esRequest);
            if (response is TOut castedResponse)
                return castedResponse;
        }

        throw new InvalidCastException();
    }

    public override int GetHashCode() => ServiceName.GetHashCode() ^ ServiceConfig.GetHashCode();
}
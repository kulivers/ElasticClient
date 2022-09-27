using Processor;

namespace ElasticClient;

[ProcessElement(nameof(ElasticProcessor), ProcessingAttributeBehaviourType.Processor)]
public class ElasticProcessor : IProcessor<EsRequest, EsResponse>
{
    private readonly EsClient _esClient;
    private readonly string? NotSupportedConfigType = $"Not supported type of config file. Now supported types: {string.Join(" ,", Enum.GetValues(typeof(ConfigType)))}";
    public string ServiceName => ProcessorConfig.Name;
    public ProcessorConfig ProcessorConfig { get; }

    public ElasticProcessor(ProcessorConfig config)
    {
        if (config.ConfigType != ConfigType.Yaml)
        {
            throw new NotSupportedException(NotSupportedConfigType);
        }

        ProcessorConfig = config;
        var clientConfig = EsClientConfig.FromYaml(config.Config);
        _esClient = new EsClient(clientConfig);
    }

    public async Task CheckHealth()
    {
        await _esClient.CheckElasticAvailable();
    }

    public EsResponse Process(EsRequest value)
    {
        return _esClient.WriteRecord(value);
    }

    public async Task<EsResponse> ProcessAsync(EsRequest value)
    {
        return await _esClient.WriteRecordAsync(value);
    }

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

    public override int GetHashCode() => ServiceName.GetHashCode() ^ ProcessorConfig.GetHashCode();
}
using Localization.Processors;
using Processor;

namespace ElasticClient;

[ProcessElement(nameof(ElasticProcessor), ProcessingAttributeBehaviourType.Processor)]
public class ElasticProcessor : IProcessor<EsRequest, EsResponse>
{
    private readonly EsClient _esClient;
    private readonly string NotSupportedConfigType = ProcessorsResources.NotSupportedConfigType;
    public string Name => ProcessorConfig.Name;
    public ProcessorConfig ProcessorConfig { get; }
    private double SecondsToResponse => 5;

    public ElasticProcessor(ProcessorConfig config)
    {
        if (config.ConfigType != ConfigType.Yaml)
        {
            var possibleConfigTypes = string.Join(", ", Enum.GetValues(typeof(ConfigType)));
            var exInfo = string.Format(NotSupportedConfigType, possibleConfigTypes);
            throw new NotSupportedException(exInfo);
        }

        ProcessorConfig = config;
        var clientConfig = EsClientConfig.FromYaml(config.Config);
        _esClient = new EsClient(clientConfig);
    }

    public async void CheckHealth()
    {
        await _esClient.CheckElasticAvailable(SecondsToResponse);
    }

    public EsResponse Process(EsRequest value, CancellationToken token)
    {
        return _esClient.WriteRecord(value, token);
    }

    public async Task<EsResponse> ProcessAsync(EsRequest value, CancellationToken token)
    {
        return await _esClient.WriteRecordAsync(value, token);
    }

    public TOut Process<TIn, TOut>(TIn value, CancellationToken token)
    {
        if (value is EsRequest esRequest)
        {
            var response = Process(esRequest,token);
            if (response is TOut castedResponse)
                return castedResponse;
        }

        throw new InvalidCastException();
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode() ^ ProcessorConfig.GetHashCode();
    }
}
using ElasticClient;
using Processor;

namespace Processors;

[ProcessElement(nameof(ElasticProcessorFactory), ProcessingAttributeBehaviourType.Factory)]
public class ElasticProcessorFactory : IProcessorFactory<EsRequest, EsResponse>
{
    public IProcessor<EsRequest, EsResponse> GetOrCreateService(ServiceConfig config) => new ElasticProcessor(config);
}

using ElasticClient;
using Processor;

namespace Processors;

[ProcessElement(nameof(ElasticProcessorFactory), ProcessingAttributeBehaviourType.Factory)]
public class ElasticProcessorFactory : IProcessorFactory<EsRequest, EsResponse>
{
    private HashSet<IProcessor> Processors { get; }

    public ElasticProcessorFactory()
    {
        Processors = new HashSet<IProcessor>();
    }

    public IProcessor GetOrCreateProcessor(ProcessorConfig config)
    {
        var saved = Processors.FirstOrDefault(p => Equals(p.ProcessorConfig, config));
        if (saved!=null)
            return saved;
        var elasticProcessor = new ElasticProcessor(config);
        Processors.Add(elasticProcessor);
        return elasticProcessor;
    }
}
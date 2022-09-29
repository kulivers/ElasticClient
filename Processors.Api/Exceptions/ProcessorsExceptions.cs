using Localization.Libs;

namespace Processor.Api.Exceptions;

public class TooBigDelayFromElasticException : Exception
{
    private static readonly string TooBigDelayFromElastic = ElasticClientResources.TooBigDelayFromElastic;
    public TooBigDelayFromElasticException(string host) : base(string.Format(TooBigDelayFromElastic, host))
    {
        
    }
}
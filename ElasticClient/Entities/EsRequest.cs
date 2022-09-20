using System.Text;
using ElasticClient;

[ProcessElement(nameof(EsRequest), ProcessingAttributeBehaviourType.Input)]
public class EsRequest
{
    private HostConfig HostConfig { get; }

    private RequestParameters RequestParameters { get; }
    private string Data { get; }

    public EsRequest(HostConfig hostConfig, RequestParameters requestParameters, string data)
    {
        HostConfig = hostConfig;
        RequestParameters = requestParameters;
        Data = data;
    }

    private Uri BuildUri() => BuildUri(HostConfig, RequestParameters);

    private Uri BuildUri(HostConfig host, RequestParameters request)
    {
        return request.DocId != null
            ? new Uri($"{host.Scheme}://{host.Host}:{host.Port}/{request.Index}/{request.Type}/{request.DocId}")
            : new Uri($"{host.Scheme}://{host.Host}:{host.Port}/{request.Index}/{request.Type}");
    }

    public HttpRequestMessage ToHttpRequestMessage()
    {
        var method = RequestParameters.DocId == null ? HttpMethod.Post : HttpMethod.Put;
        var uri = BuildUri();
        return new HttpRequestMessage(method, uri)
        {
            Content = new StringContent(Data, Encoding.UTF8, "application/json"),
        };
    }
}



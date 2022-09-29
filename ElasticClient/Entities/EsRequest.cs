using System.Text;
using ElasticClient;

[ProcessElement(nameof(EsRequest), ProcessingAttributeBehaviourType.Input)] 
public class EsRequest
{
    public HostConfig HostConfig { get; set; }
    public RequestParameters RequestParameters { get; set; }
    public string Data { get; set; }

    
    public EsRequest(HostConfig hostConfig, RequestParameters requestParameters, string data)
    {
        HostConfig = hostConfig;
        RequestParameters = requestParameters;
        Data = data;
    }

    private Uri BuildUri() => BuildUri(HostConfig, RequestParameters);

    private Uri BuildUri(HostConfig host, RequestParameters request)
    {
        if (request.DocId != null)
        {
            return new Uri($"{host.Scheme}://{host.Host}:{host.Port}/{request.Index}/{request.Type}/{request.DocId}");
        }
        return new Uri($"{host.Scheme}://{host.Host}:{host.Port}/{request.Index}/{request.Type}");
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
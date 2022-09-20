using System.Net.Http.Headers;
using ElasticClient;
using ElasticClient.Extensions;

public class EsClient
{
    private HostConfig HostConfig { get; }
    private IAuthenticationCredentials? AuthCredentials { get; }
    public EsClient(EsClientConfig esClientConfig)
    {
        HostConfig = esClientConfig.Host;
        AuthCredentials = esClientConfig.GetAuthCredentials();
    }

    public EsClient(HostConfig hostConfig, IAuthenticationCredentials? authCredentials = null)
    {
        HostConfig = hostConfig;
        AuthCredentials = authCredentials;
    }
    private HttpClient Client
    {
        get
        {
            var httpClient = new HttpClient();
            if (AuthCredentials != null)
                httpClient.DefaultRequestHeaders.Add("Authorization", AuthCredentials.ToHeaderValue());
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header
            return httpClient;
        }
    }

    public EsResponse WriteRecord(string index, CancellationToken token, string data, string? docId = null,
        string? type = "_doc")
    {
        var parameters = new RequestParameters(index, type, docId);
        return WriteRecord(parameters, token, data);
    }

    public EsResponse WriteRecord(RequestParameters requestParameters, CancellationToken token,
        string data)
    {
        var request = new EsRequest(HostConfig, requestParameters, data);
        return WriteRecord(request, token);
    }

    public EsResponse WriteRecord(EsRequest esRequest, CancellationToken token = default)
    {
        try
        {
            return Client.Send(esRequest.ToHttpRequestMessage(), token).ToEsResponse().Result;
        }
        catch (Exception e)
        {
            return new EsResponse(false, null, e.Message);
        }
    }

    public Task<EsResponse> WriteRecordAsync(string index, CancellationToken token, string? data = null,
        string? docId = null,
        string? type = "_doc")
    {
        var parameters = new RequestParameters(index, type, docId);
        return WriteRecordAsync(parameters, token, data);
    }

    public async Task<EsResponse> WriteRecordAsync(RequestParameters requestParameters, CancellationToken token,
        string? data = null)
    {
        var request = new EsRequest(HostConfig, requestParameters, data);
        return await WriteRecordAsync(request, token);
    }

    private async Task<EsResponse> WriteRecordAsync(EsRequest esRequest, CancellationToken token)
    {
        try
        {
            return await (await Client.SendAsync(esRequest.ToHttpRequestMessage(), token)).ToEsResponse();
        }
        catch (Exception e)
        {
            return new EsResponse(false, null, e.Message);
        }
    }
}
using System.Net;
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

    public async Task CheckElasticAvailable(int secondsToResponse)
    {
        var cts = new CancellationTokenSource(new TimeSpan(secondsToResponse));
        var responseMessage = await Client.GetAsync(new Uri($"https://{HostConfig.Host}:{HostConfig.Port}/_cat/health"), cts.Token);
        if (responseMessage == null)
            throw new HttpRequestException($"ElasticSearch server {HostConfig.Host}:{HostConfig.Port} is not available");
        if (!responseMessage.IsSuccessStatusCode)
        {
            var response = await responseMessage.Content.ReadAsStringAsync();
            throw new HttpRequestException($"ElasticSearch server is not healthy. Output: {response}"); //todo ask
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
            var requestMessage = esRequest.ToHttpRequestMessage();
            var result = Client.Send(requestMessage, token).ToEsResponseAsync().Result;
            return result;
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

    public async Task<EsResponse> WriteRecordAsync(EsRequest esRequest, CancellationToken token = default)
    {
        try
        {
            return await (await Client.SendAsync(esRequest.ToHttpRequestMessage(), token)).ToEsResponseAsync();
        }
        catch (Exception e)
        {
            return new EsResponse(false, null, e.Message);
        }
    }
}
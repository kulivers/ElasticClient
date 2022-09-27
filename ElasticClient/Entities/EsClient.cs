using System.Net.Http.Headers;
using ElasticClient;
using ElasticClient.Extensions;

public class EsClient
{
    private readonly string ElasticSearchNotAvailable = $"ElasticSearch server {0}:{1} is not available"; //loc
    private readonly string ElasticSearchNotHealthy = $"ElasticSearch server is not healthy. Output: \n {0}"; //loc
    private const string AuthorizationHeaderKey = "Authorization";
    private HostConfig HostConfig { get; }
    private IAuthenticationCredentials? AuthCredentials { get; }

    public EsClient(EsClientConfig esClientConfig)
    {
        HostConfig = esClientConfig.Host;
        AuthCredentials = esClientConfig.GetAuthCredentials();
    }

    private HttpClient Client
    {
        get
        {
            var httpClient = new HttpClient();

            if (AuthCredentials != null)
            {
                httpClient.DefaultRequestHeaders.Add(AuthorizationHeaderKey, AuthCredentials.ToHeaderValue());
            }

            var acceptHeader = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(acceptHeader); //ACCEPT header
            return httpClient;
        }
    }

    public async Task CheckElasticAvailable()
    {
        var requestIri = new Uri($"https://{HostConfig.Host}:{HostConfig.Port}/_cat/health");
        var delay = new TimeSpan(0, 0, 20);
        var cts = new CancellationTokenSource(delay);
        var responseMessage = await Client.GetAsync(requestIri, cts.Token);

        if (responseMessage == null)
        {
            throw new HttpRequestException(string.Format(ElasticSearchNotAvailable, HostConfig.Host, HostConfig.Port));
        }

        if (!responseMessage.IsSuccessStatusCode)
        {
            var responseContent = await responseMessage.Content.ReadAsStringAsync(CancellationToken.None);
            throw new HttpRequestException(string.Format(ElasticSearchNotHealthy, responseContent));
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
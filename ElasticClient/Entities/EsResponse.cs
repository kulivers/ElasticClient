using System.Net;
using ElasticClient;

[ProcessElement(nameof(EsResponse), ProcessingAttributeBehaviourType.Output)]
public class EsResponse
{
    public bool Success { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public string Data { get; set; }

    public EsResponse(bool success, HttpStatusCode? statusCode, string data)
    {
        Success = success;
        StatusCode = statusCode;
        Data = data;
    }
}
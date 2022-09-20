namespace ElasticClient;

public class RequestParameters
{
    public string Index { get; set; }
    public string? Type { get; set; }
    public string? DocId { get; set; }

    public RequestParameters(string index, string? type = "_doc", string? id = null)
    {
        Index = index;
        Type = type;
        DocId = id;
    }
}
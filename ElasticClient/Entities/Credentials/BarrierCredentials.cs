namespace ElasticClient.Entities;

public class BarrierCredentials : AuthenticationCredentials
{
    public string Type => "Barrier";
    public string Token { get; set; }

    public BarrierCredentials(string token)
    {
        Token = token;
    }
}
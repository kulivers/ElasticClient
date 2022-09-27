namespace ElasticClient.Entities;

public class BarrierCredentials : AuthenticationCredentials
{
    public override string Type => "Barrier";
    public override string Token { get; set; }

    public BarrierCredentials(string token)
    {
        Token = token;
    }
}
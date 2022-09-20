namespace ElasticClient.Entities;

public class BarrierCredentials : IAuthenticationCredentials
{
    public string Type => "Barrier";
    public string Token { get; set; }

    public BarrierCredentials(string token)
    {
        Token = token;
    }

    public string ToHeaderValue() => $"{Type} {Token}";

}
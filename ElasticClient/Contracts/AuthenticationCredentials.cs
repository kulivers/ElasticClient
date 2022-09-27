namespace ElasticClient;

public abstract class AuthenticationCredentials
{
    public string Type { get;}
    public string Token { get; set; }
    public string ToHeaderValue() => $"{Type} {Token}";
}
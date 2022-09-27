namespace ElasticClient;

public abstract class AuthenticationCredentials
{
    public abstract string Type { get;}
    public abstract string Token { get; set; }
    public string ToHeaderValue() => $"{Type} {Token}";
}
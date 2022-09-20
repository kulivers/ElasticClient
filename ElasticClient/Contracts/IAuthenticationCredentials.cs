

namespace ElasticClient;

public interface IAuthenticationCredentials
{
    public string Type { get;}
    public string Token { get; set; }
    public string ToHeaderValue();
    

}
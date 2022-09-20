using System.Text;

namespace ElasticClient.Entities;

public class BasicCredentials : IAuthenticationCredentials
{
    public string Type => "Basic";
    public string Token { get; set; }

    public BasicCredentials(string token)
    {
        Token = token;
    }
    public BasicCredentials(string userName, string password)
    {
        Token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
    }

    public string ToHeaderValue() => $"{Type} {Token}";

}
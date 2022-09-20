using System.Text;

namespace ElasticClient.Entities;

public class ApiKeyCredentials : IAuthenticationCredentials
{
    public string Type => "ApiKey";
    public string Token { get; set; }

    public ApiKeyCredentials(string token)
    {
        Token = token;
    }
    public ApiKeyCredentials(string userName, string password)
    {
        Token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
    }

    public string ToHeaderValue() => $"{Type} {Token}";

}
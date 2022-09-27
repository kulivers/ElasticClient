using System.Text;

namespace ElasticClient.Entities;

public class ApiKeyCredentials : AuthenticationCredentials
{
    public string Type => "ApiKey";
    public string Token { get; set; }

    public ApiKeyCredentials(string token)
    {
        Token = token;
    }
    public ApiKeyCredentials(string userName, string password)
    {
        var bytes = Encoding.UTF8.GetBytes($"{userName}:{password}");
        Token = Convert.ToBase64String(bytes);
    }
    
}
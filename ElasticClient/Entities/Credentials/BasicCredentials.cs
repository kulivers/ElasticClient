using System.Text;

namespace ElasticClient.Entities;

public class BasicCredentials : AuthenticationCredentials
{
    public string Type => "Basic";
    public string Token { get; set; }

    public BasicCredentials(string token)
    {
        Token = token;
    }
    public BasicCredentials(string userName, string password)
    {
        var bytes = Encoding.UTF8.GetBytes($"{userName}:{password}");
        Token = Convert.ToBase64String(bytes);
    }
}
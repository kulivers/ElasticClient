namespace ElasticClient;

public class AuthenticationConfig 
{
    public AuthenticationConfig(string type, string username, string password)
    {
        Type = type;
        Username = username;
        Password = password;
    }
    public AuthenticationConfig(string type, string token)
    {
        Type = type;
        Token = token;
    }

    public AuthenticationConfig()
    {
        
    }
    
    public string Type { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }

    
}
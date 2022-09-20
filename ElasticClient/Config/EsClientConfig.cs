using ElasticClient.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ElasticClient;

public class EsClientConfig
{
    public static EsClientConfig GetFromFile(string filePath)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) //todo egor mb it is not camel case
            .Build();
        var fileContent = File.ReadAllText(filePath);
        
        return deserializer.Deserialize<EsClientConfig>(fileContent);
    }


    public HostConfig Host { get; set; }
    public AuthenticationConfig Authentication { get; set; }

    public EsClientConfig()
    {
    }

    public EsClientConfig(HostConfig host, AuthenticationConfig authentication)
    {
        Host = host;
        Authentication = authentication;
    }

    public IAuthenticationCredentials GetAuthCredentials()
    {
        return Authentication.Type.ToUpper() switch
        {
            "BASIC" => Authentication.Token == null
                ? new BasicCredentials(Authentication.Username, Authentication.Password)
                : new BasicCredentials(Authentication.Token),
            "APIKEY" => Authentication.Token == null
                ? new ApiKeyCredentials(Authentication.Username, Authentication.Password)
                : new ApiKeyCredentials(Authentication.Token),
            "BARRIER" => new BarrierCredentials(Authentication.Token),
            "OAUTH" => new BarrierCredentials(Authentication.Token),
            _ => throw new Exception("Unknown authentication type")
        };
    }
}
using ElasticClient.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ElasticClient;

public class EsClientConfig
{
    public static EsClientConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
            throw new ArgumentException("wrong type of file. need to be .yaml");
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) //todo egor mb it is not camel case
            .Build();
        var fileContent = File.ReadAllText(path);
        
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
        switch (Authentication.Type.ToUpper())
        {
            case "BASIC":
            {
                return Authentication.Token == null
                    ? new BasicCredentials(Authentication.Username!, Authentication.Password!)
                    : new BasicCredentials(Authentication.Token);
            }
            case "APIKEY":
            {
                return Authentication.Token == null
                    ? new ApiKeyCredentials(Authentication.Username!, Authentication.Password!)
                    : new ApiKeyCredentials(Authentication.Token);
            }
            case "BARRIER":
            case "OAUTH":
            {
                return new BarrierCredentials(Authentication.Token!);
            }
            default:
            {
                throw new Exception("Unknown authentication type");
            }
        }
    }
}
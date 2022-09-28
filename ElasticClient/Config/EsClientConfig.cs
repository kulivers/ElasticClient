using ElasticClient.Entities;
using Localization.Libs;
using Utils;

namespace ElasticClient;

public class EsClientConfig
{
    private static readonly string UnknownAuthenticationType = ElasticClientResources.UnknownAuthenticationType;
    private const string Basic = "BASIC";
    private const string ApiKey = "APIKEY";
    private const string Barrier = "BARRIER";
    private const string OAuth = "OAUTH";
    

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

    public AuthenticationCredentials GetAuthCredentials()
    {
        switch (Authentication.Type.ToUpper())
        {
            case Basic:
            {
                return Authentication.Token == null
                    ? new BasicCredentials(Authentication.Username!, Authentication.Password!)
                    : new BasicCredentials(Authentication.Token);
            }
            case ApiKey:
            {
                return Authentication.Token == null
                    ? new ApiKeyCredentials(Authentication.Username!, Authentication.Password!)
                    : new ApiKeyCredentials(Authentication.Token);
            }
            case Barrier:
            case OAuth:
            {
                return new BarrierCredentials(Authentication.Token!);
            }
            default:
            {
                throw new NotSupportedException(UnknownAuthenticationType);
            }
        }
    }
    public static EsClientConfig FromYaml(string path)
    {
        return YamlConfigHelper.GetConfigFromCamelYaml<EsClientConfig>(path);
    }

}
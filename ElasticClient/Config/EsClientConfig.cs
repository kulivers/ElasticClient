using ElasticClient.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ElasticClient;

public class EsClientConfig
{
    private const string WrongTypeOfFileNeedToBeYaml = "wrong type of file. need to be .yaml";
    private const string UnknownAuthenticationType = "Unknown authentication type";
    private const string Basic = "BASIC";
    private const string ApiKey = "APIKEY";
    private const string Barrier = "BARRIER";
    private const string OAuth = "OAUTH";

    public static EsClientConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
        {
            throw new ArgumentException(WrongTypeOfFileNeedToBeYaml);
        }

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
                throw new Exception(UnknownAuthenticationType);
            }
        }
    }
}
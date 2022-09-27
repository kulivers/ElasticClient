namespace ElasticClient;

public class HostConfig 
{
    public string Scheme { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }

    public HostConfig()
    {
        //for mapping from config file    
    }

    public HostConfig(string host, int port, string scheme = "https")
    {
        Scheme = scheme;
        Host = host;
        Port = port;
    }
}
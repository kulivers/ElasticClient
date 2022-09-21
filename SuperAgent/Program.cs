using Processor;
using ProcessorsRunner;

class Program
{
    public static void Main(string[] args)
    {
        // var servicesYaml = "D:\\Work\\ElasticClient\\SuperAgent\\config\\services.yaml";
        // var connectorsYaml = "D:\\Work\\ElasticClient\\SuperAgent\\config\\connectors.yaml";
        // var servicesConfig = ServicesConfig.FromYaml(servicesYaml);
        
        var agentYaml = "D:\\Work\\ElasticClient\\SuperAgent\\config\\agent.yaml";
        var cfg = AgentConfig.FromYaml(agentYaml);
        var agent = new SuperAgent(cfg);
        agent.Start();
    }

    public static string GetMock() =>
        "{\"HostConfig\":{\"Scheme\":\"https\",\"Host\":\"localhost\",\"Port\":9200},\"RequestParameters\":" +
        "{\"Index\":\"test3\",\"Type\":\"_doc\",\"DocId\":null}," +
        "\"Data\":\"{\\\"name\\\": 312}\"}";
}
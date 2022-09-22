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
}
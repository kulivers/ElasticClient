using Processor;
using ProcessorsRunner;

class Program
{
    public static void Main(string[] args)
    {
        var agentYaml = "D:\\Work\\myProcessorAgent\\SuperAgent\\config\\agent.yaml";
        var cfg = AgentConfig.FromYaml(agentYaml);
        var agent = new SuperAgent(cfg);
        agent.Start();
    }
}
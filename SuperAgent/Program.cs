using Autofac.Core.Registration;
using Processor;
using ProcessorsRunner;
using YamlDotNet.Serialization.NodeTypeResolvers;

class Program
{

    public static async Task Main(string[] args)
    {
        var agentYaml = "D:\\Work\\myProcessorAgent\\SuperAgent\\config\\agent.yaml";
        var cfg = AgentConfig.FromYaml(agentYaml);
        var agent = new SuperAgent(cfg);
        await agent.Start();
    }
}
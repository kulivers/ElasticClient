using Processor;

namespace InputServices.Test;

public class ProcessorsTestsHelper
{
    public ProcessorContainer GetProcessorContainerWithElastic(string serviceName)
    {
        var mockDll = "D:\\Work\\myProcessorAgent\\TestHelpers\\MockFiles\\ElasticProcessor.dll";
        var processorConfig = new ProcessorConfig()
        {
            Config = "D:\\Work\\myProcessorAgent\\TestHelpers\\MockFiles\\processor-elastic.yaml",
            Dll = mockDll,
            Name = serviceName
        };
        var processorConfigsList = new List<ProcessorConfig>() { processorConfig };
        var processorsConfigs = new ProcessorsConfigs(processorConfigsList);
        return new ProcessorContainer(processorsConfigs);
    }
}
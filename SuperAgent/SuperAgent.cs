using System.Collections.Concurrent;
using Processor;
using ProcessorsRunner;

public class SuperAgent
{
    private readonly ServicesConfig _servicesConfig;
    private readonly ConnectorsConfig _connectorsConfig;
    private readonly QueueThrottler _queueThrottler;
    public IProcessorsContainer ProcessorsContainer { get; }
    public List<IConnector> Connectors { get; set; }

    public SuperAgent(AgentConfig agentConfig) : this(new ServicesConfig(agentConfig.Services),
        new ConnectorsConfig(agentConfig.Connectors))
    {
    }

    public SuperAgent(ServicesConfig servicesConfig, ConnectorsConfig connectorsConfig)
    {
        _servicesConfig = servicesConfig;
        _connectorsConfig = connectorsConfig;
        ProcessorsContainer = new ProcessorContainer(servicesConfig);
        _queueThrottler = new QueueThrottler(100);
        InitConnectors(connectorsConfig);
        ThrowIfConfigsNotValid();
        CheckHealth();
    }

    private void InitConnectors(ConnectorsConfig connectorsConfig)
    {
        Connectors = new List<IConnector>();
        foreach (var connectorConfig in connectorsConfig.Connectors)
        {
            if (connectorConfig.IoServiceType == IoType.Kafka)
            {
                Connectors.Add(new KafkaConnector(connectorConfig));
            }
        }
    }

    public async Task Start()
    {
        await MapConnectors();
    }

    private async Task MapConnectors()
    {
        // foreach (var connector in Connectors)
        // {
        //     connector.OnReceive += (_, data) =>
        //     {
        //         //above we can handle responses from specified processors to specified connectors 
        //         var response = ProcessorsContainer.Process(connector.DestinationService, data);
        //     };
        //     await connector.StartReceive(CancellationToken.None);
        // }

        //todo delete above, this it example /////////////////////////////////////////////////


        Task.Run(() => WaitTask().Start());
        foreach (var connector in Connectors)
        {
            connector.OnReceive += (_, data) =>
            {
                var processFunc = () => ProcessorsContainer.Process(connector.DestinationService, data);
                var task = new Task<object?>(processFunc);
                _queueThrottler.Add(task);
            };
            connector.StartReceive(CancellationToken.None).Start();
        }
    }

    private async Task WaitTask()
    {
        var i = 0;
        while (true)
        {
            await _queueThrottler.WaitFirstTaskCompleted()
                .ContinueWith(o =>
                {
                    i++;
                    if (i / 1000 == 1)
                        Console.WriteLine(i);
                });
        }
    }


    private void ThrowIfConfigsNotValid()
    {
        ValidateServicesConfig();
        ValidateConnectorsConfig();

        void ValidateServicesConfig()
        {
            foreach (var config in _servicesConfig.Services)
            {
                File.Open(config.Dll, FileMode.Open, FileAccess.Read).Dispose();
                File.Open(config.Config, FileMode.Open, FileAccess.Read).Dispose();
            }
        }

        void ValidateConnectorsConfig()
        {
            foreach (var config in _connectorsConfig.Connectors)
            {
                if (!_servicesConfig.Services.Any(cfg => cfg.ServiceName == config.Destination))
                    throw new ApplicationException(
                        $"There is no service {config.Destination} for {config.IoServiceType} connector");
                if (config.IoServiceType == IoType.Kafka)
                {
                    var path = config.Properties["config"];
                    File.Open(path, FileMode.Open, FileAccess.Read).Dispose();
                }
            }
        }
    }

    private void CheckHealth()
    {
        foreach (var processor in ProcessorsContainer)
        {
            processor.CheckHealth();
        }

        foreach (var connector in Connectors)
        {
            connector.CheckHealth();
        }
    }
}
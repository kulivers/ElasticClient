using InputServices;
using IOServices.Api;
using Localization;
using OuputServices;

namespace ProcessorsRunner;

public class ConnectorFactory
{
    private static readonly string PossibleInputTypes = string.Join(", ", Enum.GetValues(typeof(InputService)));
    private static readonly string InputServiceCantBeNull = string.Format(ConnectorResources.InputServiceCantBeNull, PossibleInputTypes);
    private static readonly string OutputConfigCantBeNull = ConnectorResources.OutputConfigCantBeNull;

    public IConnector CreateConnector(ConnectorConfig config)
    {
        var destination = config.Destination;
        IInputService? inputService = null;
        IOutputService? outputService = null;

        if (config.Input == InputService.Kafka)
        {
            var inputConfig = new KafkaInputConfig(config.InputConfig);
            inputService = new KafkaInput(inputConfig);
        }

        if (config.Output == OutputService.Kafka)
        {
            if (config.OutputConfig == null)
            {
                throw new InvalidOperationException(OutputConfigCantBeNull);
            }
            
            var outputConfig = new KafkaOutputConfig(config.OutputConfig);
            outputService = new KafkaOutputService(outputConfig);
        }

        if (inputService == null)
        {
            throw new InvalidOperationException(InputServiceCantBeNull);
        }

        return new Connector(destination, inputService, outputService);
    }
}
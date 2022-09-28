using Confluent.Kafka;
using InputServices;
using InputServices.Test;
using OuputServices;

namespace OutputServices.Test;

public class SendingTests
{
    public KafkaTestsHelper KafkaTestsHelper { get; set; }
    public IAdminClient AdminClient { get; set; }

    [SetUp]
    public void Setup()
    {
        KafkaTestsHelper = new KafkaTestsHelper();
        AdminClient = KafkaTestsHelper.GetAdminClient();
    }

    [Test]
    public async Task DontThrowIfBigDelayOnSend()
    {// Arrange
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4));
        var topic = "randomTopicName123232";
        var topics = new[] { topic };
        await KafkaTestsHelper.CreateTopicAsync(topic,cts.Token);

        var kafkaOutputConfig = new KafkaOutputConfig()
        {
            Client = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.BadBootstrapServers),
            Topics = topics
        };
        var kafkaOutputService = new KafkaOutputService(kafkaOutputConfig);
        var fastCts = new CancellationTokenSource(0);

        //Assert
        Assert.DoesNotThrowAsync(async () =>
        {
             await kafkaOutputService.Send("1", fastCts.Token);
        });
        
        var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(4));
        await KafkaTestsHelper.DeleteTopicAsync(topic, cts2.Token);
    }
}
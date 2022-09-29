using Confluent.Kafka;
using IOServices.Api;

namespace InputServices.Test;

public class ReceivingTests
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
    public async Task ReturnsGoodResponses()
    {
        //Arrange
        var producer = KafkaTestsHelper.GetStringProducer();
        var topicName = "SomeRandomTopic123121";
        var config = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.GoodBootstrapServers);
        var inputConfig = new KafkaInputConfig() { Client = config, Topics = new[] { topicName } };
        var inputService = new KafkaInput(inputConfig);
        var toSend = new Message<int, string>() { Value = "Some value" };
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(100));

        //Act
        await KafkaTestsHelper.CreateTopicAsync(topicName, cts.Token);
        Task.Run(async () => await inputService.StartReceive(CancellationToken.None));
        InputModel receivedModel = null;
        inputService.OnReceive += (sender, model) => { receivedModel = model; };
        await producer.ProduceAsync(topicName, toSend, cts.Token);

        while (receivedModel == null)
        {
            if (cts.Token.IsCancellationRequested)
            {
                cts.Token.ThrowIfCancellationRequested();
            }
        }

        //Assert
        Assert.That(receivedModel?.Data, Is.EqualTo(toSend.Value));

        //Cleaning
        var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(4));
        await KafkaTestsHelper.DeleteTopicAsync(topicName, cts2.Token);
    }
}
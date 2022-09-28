using Confluent.Kafka;
using Confluent.Kafka.Admin;
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
        if (!KafkaTestsHelper.IsServerAvailable())
        {
            return;
        }

        //Arrange
        var producer = KafkaTestsHelper.GetStringProducer();
        var topicName = "SomeRandomTopic123121";
        var config = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.GoodBootstrapServers);
        var inputConfig = new KafkaInputConfig() { Client = config, Topics = new[] { topicName } };
        var inputService = new KafkaInputService(inputConfig);
        var toSend = new Message<int, string>() { Value = "Some value" };

        //Act
        await KafkaTestsHelper.CreateTopicAsync(topicName);
        Task.Run(async () => await inputService.StartReceive(CancellationToken.None));
        InputResponseModel receivedModel = null;
        inputService.OnReceive += (sender, model) => { receivedModel = model; };
        var deliveryResult = await producer.ProduceAsync(topicName, toSend);

        while (!inputService.ReceivedAny)
        {
            
        }
        //Assert
        Assert.That(receivedModel?.Data, Is.EqualTo(toSend.Value));

        //Cleaning
        await KafkaTestsHelper.DeleteTopicAsync(topicName);
    }
}
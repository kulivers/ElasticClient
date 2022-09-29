using Confluent.Kafka;

namespace InputServices.Test;

public class HealthCheckTests
{
    public KafkaTestsHelper KafkaTestsHelper => new KafkaTestsHelper();
    public IAdminClient AdminClient { get; set; }

    [SetUp]
    public void Setup()
    {
        AdminClient = KafkaTestsHelper.GetAdminClient();
    }

    [Test]
    public void ThrowsIfBadPort()
    {
        // Arrange
        var badConfig = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.BadBootstrapServers);
        var badInputConfig = new KafkaInputConfig() { Client = badConfig, Topics = KafkaTestsHelper.InputTopics };
        var kafkaInputService = new KafkaInput(badInputConfig);
        Assert.Throws<KafkaException>(() => kafkaInputService.CheckHealth(4));
    }

    [Test]
    public void ThrowsIfBigDelay()
    {
        // Arrange
        var badInputConfig = new KafkaInputConfig() { Client = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.GoodBootstrapServers), Topics = KafkaTestsHelper.InputTopics };
        var kafkaInputService = new KafkaInput(badInputConfig);
        Assert.Throws<KafkaException>(() => kafkaInputService.CheckHealth(0));
    }

    [Test]
    public void HealthCheckDoesntThrows()
    {
        // Arrange
        var goodClientConfig = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.GoodBootstrapServers);
        var goodConfig = new KafkaInputConfig() { Client = goodClientConfig, Topics = KafkaTestsHelper.InputTopics };
        var kafkaInputService = new KafkaInput(goodConfig);
        Assert.DoesNotThrow(() => kafkaInputService.CheckHealth(4));
    }

    [Test]
    public async Task ThrowsIfBadTopics()
    {
        // Arrange
        var randomTopicName = "SomeRandomName13131931";
        var mockTopics = new List<string>() { randomTopicName };
        var clientGoodConfig = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.GoodBootstrapServers);
        var configWithBadTopics = new KafkaInputConfig() { Client = clientGoodConfig, Topics = mockTopics };
        var kafkaInputService = new KafkaInput(configWithBadTopics);
        
        //Assert
        Assert.Throws<IOException>(() => kafkaInputService.CheckHealth(4));
        
        //if kafka config has auto creating topics => delete after it was created 
        try
        {
            var metadata = AdminClient.GetMetadata(randomTopicName, TimeSpan.FromSeconds(3));
            if (metadata.Topics.First().Error.IsError == false)
            {
                await AdminClient.DeleteTopicsAsync(new[] { randomTopicName });
            }
        }
        catch
        {
            // ignored
        }
    }
}
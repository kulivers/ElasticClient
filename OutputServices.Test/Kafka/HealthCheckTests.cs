using Confluent.Kafka;
using OuputServices;

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
        var badConfig = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.BadBootstrapServers);
        var badInputConfig = new KafkaOutputConfig() { Client = badConfig, Topics = KafkaTestsHelper.InputTopics };
        var kafkaOutputService = new KafkaOutputService(badInputConfig);
        Assert.Throws<KafkaException>(() => kafkaOutputService.CheckHealth(4));
    }

    [Test]
    public void ThrowsIfBigDelay()
    {
        var clientConfig = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.GoodBootstrapServers);
        var badInputConfig = new KafkaOutputConfig() { Client = clientConfig, Topics = KafkaTestsHelper.InputTopics };
        var kafkaOutputService = new KafkaOutputService(badInputConfig);
        Assert.Throws<KafkaException>(() => kafkaOutputService.CheckHealth(0));
    }

    [Test]
    public void HealthCheckDoesntThrows()
    {
        var goodClientConfig = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.GoodBootstrapServers);
        var goodConfig = new KafkaOutputConfig() { Client = goodClientConfig, Topics = KafkaTestsHelper.InputTopics };
        var kafkaOutputService = new KafkaOutputService(goodConfig);
        if (KafkaTestsHelper.IsServerAvailable())
        {
            Assert.DoesNotThrow(() => kafkaOutputService.CheckHealth(4));
        }
        else
        {
            Assert.Throws<KafkaException>(() => kafkaOutputService.CheckHealth(4));
        }
    }

    [Test]
    public async Task ThrowsIfBadTopics()
    {
        var randomTopicName = "SomeRandomName13131931";
        var mockTopics = new List<string>() { randomTopicName };
        var clientGoodConfig = KafkaTestsHelper.GetClientConfig(KafkaTestsHelper.GoodBootstrapServers);
        var configWithBadTopics = new KafkaOutputConfig() { Client = clientGoodConfig, Topics = mockTopics };
        var kafkaInputService = new KafkaOutputService(configWithBadTopics);

        if (KafkaTestsHelper .IsServerAvailable())
        {
            Assert.Throws<IOException>(() => kafkaInputService.CheckHealth(4));
        }
        else
        {
            Assert.Throws<KafkaException>(() => kafkaInputService.CheckHealth(4));
        }


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
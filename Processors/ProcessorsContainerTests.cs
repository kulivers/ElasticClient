using System.Net;
using ElasticClient;
using InputServices.Test;
using Newtonsoft.Json;

namespace Processors;

public class ProcessorsContainerTests
{
    public ProcessorsTestsHelper Helper { get; private set; }
    private HostConfig GoodConfig => new HostConfig("localhost", 9200);


    [SetUp]
    public void Setup()
    {
        Helper = new ProcessorsTestsHelper();
    }

    [Test]
    public void BadRequestIfBadData()
    {
        // Arrange
        var serviceName = "Elastic processor";
        var container = Helper.GetProcessorContainerWithElastic(serviceName);
        var index = "existedOrNot";
        var esRequest = new EsRequest(GoodConfig, new RequestParameters(index), "some bad data");
        var esResponse = container.Process<EsRequest, EsResponse>(serviceName, esRequest, CancellationToken.None);

        //Assert
        Assert.That(esResponse?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }


    [Test]
    public void ThrowsIfBadDataToProcessor()
    {
        // Arrange 
        var serviceName = "Elastic processor";
        var container = Helper.GetProcessorContainerWithElastic(serviceName);
        var notEsRequest = "{some bread}";

        //Assert
        Assert.Throws<JsonReaderException>(() => { container.Process(serviceName, notEsRequest, CancellationToken.None); });
    }

    [Test]
    public void DontThrowIfNoResponse()
    {
        //Arrange 
        var serviceName = "Elastic processor";
        var container = Helper.GetProcessorContainerWithElastic(serviceName);
        var notEsRequest = "{some bread}";
        var badConfig = new HostConfig("localhost", 9201);

        //Act
        var index = "mbNotExiting";
        var request = new EsRequest(badConfig, new RequestParameters(index), "some data");

        //Assert
        Assert.DoesNotThrow(() => container.Process<EsRequest, EsResponse>(serviceName, request, CancellationToken.None));
    }
}
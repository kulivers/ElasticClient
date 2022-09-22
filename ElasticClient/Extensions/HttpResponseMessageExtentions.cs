namespace ElasticClient.Extensions;

public static  class HttpResponseMessageExtentions
{
    public static async Task<EsResponse> ToEsResponseAsync(this HttpResponseMessage message)
    {
        var body = await message.Content.ReadAsStringAsync();
        return new EsResponse(message.IsSuccessStatusCode, message.StatusCode, body);
    }
}
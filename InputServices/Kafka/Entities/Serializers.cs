using System.Text;
using Confluent.Kafka;

namespace KafkaInteractor;

public class StringDeserializer : IDeserializer<string>
{
    public string Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) => Encoding.UTF8.GetString(data);
}
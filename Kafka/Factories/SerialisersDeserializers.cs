using System.Text;
using Confluent.Kafka;

namespace KafkaInteractor;

internal class Serializers
{
    public class StringSerializer : ISerializer<string>
    {
        public byte[] Serialize(string data, SerializationContext context) => Encoding.UTF8.GetBytes(data);
    }
}
internal class Deserializers
{
    public class StringDeserializer : IDeserializer<string>
    {
        public string Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) => Encoding.UTF8.GetString(data);
    }
}
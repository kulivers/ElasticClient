using System.Text;
using Confluent.Kafka;

namespace KafkaInteractor;

public class StringSerializer : ISerializer<string>
{
    public byte[] Serialize(string data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(data);
    }
}
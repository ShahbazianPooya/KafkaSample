
using System.Text.Json;

namespace Kafka.Events
{
    public class Envelop<T> : AbstarctEnvelope where T : IEvent
    {        
        public Envelop(T payload, string topic, string origin) : base(payload.Key, topic, origin)
        {
            Payload= payload;
        }        
        public T Payload { get; private set; }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            return JsonSerializer.Serialize(this, options);
        }
    }

}

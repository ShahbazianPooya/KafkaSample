namespace Kafka.Events
{
    public abstract class AbstarctEnvelope
    {
        public string Key { get; private set; }
        public Guid MessageId { get; private set; }
        public Dictionary<string, string> MessageContext { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Topic { get; private set; }
        public string Origin { get; private set; }
        protected AbstarctEnvelope(string key, string topic, string origin)
        {
            Key = key;
            Topic = topic;
            MessageId = Guid.NewGuid();
            MessageContext = new();
            DateTime = DateTime.UtcNow;
            Origin = origin;
        }

    }

}

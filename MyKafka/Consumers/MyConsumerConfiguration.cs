using Confluent.Kafka.Admin;
using Confluent.Kafka;
using System.Reflection;
using System.Net;

namespace MyKafka.Consumers
{
    public class MyConsumerConfiguration

    {
        public ConsumerConfig Configuration { get; } = DefaultConsumerConfiguration();


        public TopicSpecification Topic { get; } = new();

        public uint? CommitOffsetsAfter { get; set; } = 0;

        public bool IgnoreUnknownMessageTypes { get; set; }

        internal HashSet<Type> IgnoredMessageTypes { get; } = new();

        private static ConsumerConfig DefaultConsumerConfiguration()
            => new()
            {
                EnableAutoCommit = true,
                EnableAutoOffsetStore = true,
                AutoCommitIntervalMs = 5000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky,
                AllowAutoCreateTopics = true,
               EnablePartitionEof = true,
                IsolationLevel = IsolationLevel.ReadCommitted,
                ClientId = Dns.GetHostName(),
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.None
            };
    }
}

using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyKafka.Producers
{
    public class MyProducerConfiguration
    {
        public TopicSpecification Topic { get; } = new();
        public ProducerConfig Configuration { get; set; } = CreateDefault();

        private static ProducerConfig CreateDefault()
            => new()
            {
                Acks = Acks.All,
                EnableDeliveryReports = true,
                RetryBackoffMs = 10,
                SocketNagleDisable = true,
                EnableIdempotence = true,
                MaxInFlight = 5,
                MessageSendMaxRetries = int.MaxValue,
                LingerMs = 5,
                BatchNumMessages = 100,
                ClientId = Dns.GetHostName(),
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.None,                
            };
    }
}

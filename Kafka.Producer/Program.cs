using Confluent.SchemaRegistry;
using MyKafka.Producers;
using Payment.Events;

namespace Kafka.Producer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var producerConfiguration = new MyProducerConfiguration();
            
            producerConfiguration.Topic.Name = "payment-events";
            producerConfiguration.Topic.NumPartitions= 3;
            producerConfiguration.Topic.ReplicationFactor= 1;
            producerConfiguration.Configuration.BootstrapServers = "172.29.67.194:9092";

            var producer = new MyProducer<PaymentCollectedEvent>(producerConfiguration);
            await producer.CreateTopicsAsync();
            Random random = new Random();
            while (true)
            {
                var evt = new PaymentCollectedEvent { Amount = random.Next(10, 1000), PaymentId = Guid.NewGuid() };
                await producer.ProduceAsync(evt);
                Console.WriteLine(evt);
                Console.ReadKey();
            }

        }
    }
}

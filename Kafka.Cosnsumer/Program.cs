using Kafka.Events;
using MyKafka.Consumers;
using MyKafka.Producers;
using Payment.Events;
using System;
using System.Reflection;
using System.Threading;

namespace Kafka.Cosnsumer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var consumerConfiguration = new MyConsumerConfiguration();

            var cancellationTokenSource = new CancellationTokenSource();

            consumerConfiguration.Topic.Name = "payment-events";
            consumerConfiguration.Topic.NumPartitions = 3;
            consumerConfiguration.Topic.ReplicationFactor = 1;
            consumerConfiguration.Configuration.BootstrapServers = "172.29.67.194:9092";
            consumerConfiguration.Configuration.GroupId = Assembly.GetExecutingAssembly().FullName;

            var consumer = new MyConsumer(consumerConfiguration, cancellationTokenSource);
            consumer.RegisterHandler(new PaymentcollectedHandler());
            await consumer.Consume();
            Console.ReadLine();
        }



    }
    public class PaymentcollectedHandler : IEventHandler<PaymentCollectedEvent>
    {
        public Task HandleAsync(Envelop<PaymentCollectedEvent> evt)
        {   
            Console.WriteLine(evt);
            return Task.CompletedTask;
        }
    }
}

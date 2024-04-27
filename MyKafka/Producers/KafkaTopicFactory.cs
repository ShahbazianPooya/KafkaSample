using Confluent.Kafka.Admin;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyKafka.Producers;

namespace MyKafka.Producers
{
    internal class KafkaTopicFactory
    {
        IEnumerable<MyProducerConfiguration> _producerConfigurations;
        private static readonly TimeSpan TopicCreationTimeout = TimeSpan.FromSeconds(30);

        public KafkaTopicFactory(MyProducerConfiguration producerFactories)
        {
            _producerConfigurations = [producerFactories];
        }

        public Task CreateTopicsAsync()
        {
            var topicTasks = _producerConfigurations
                .Select(f => new
                {
                    ClientConfig = (ClientConfig)f.Configuration,
                    f.Topic
                })                
                .GroupBy(z => z.ClientConfig.BootstrapServers)
                .Select(async clusterTopics =>
                {
                    var topics = clusterTopics
                        .Select(ct => ct.Topic)
                        .Distinct(new TopicComparer())
                        .ToList();

                    var topicNames = string.Join(",", topics.Select(t => t.Name));                

                    var clientConfig = clusterTopics.First().ClientConfig;

                    var config = new AdminClientConfig
                    {
                        BootstrapServers = clientConfig.BootstrapServers,
                        SaslMechanism = clientConfig.SaslMechanism,
                        SecurityProtocol = clientConfig.SecurityProtocol,
                        SaslUsername = clientConfig.SaslUsername,
                        SaslPassword = clientConfig.SaslPassword,
                        ClientId = clientConfig.ClientId
                    };
                    using var adminClient = new AdminClientBuilder(config).Build();

                    try
                    {
                        await adminClient.CreateTopicsAsync(topics,
                            new CreateTopicsOptions { OperationTimeout = TopicCreationTimeout });
                    }
                    catch (CreateTopicsException ex)
                    {
                        // propagate the exception in case all errors indicate that topics already exists.
                        if (ex.Results
                            .Select(r => r.Error.Code)
                            .Where(el => el != ErrorCode.NoError)
                            .Any(el => el != ErrorCode.TopicAlreadyExists))
                        {
                            var reasons = string.Join(",", ex.Results.Select(r => r.Error.Reason));
                                          throw;
                        }
                    }                    
                });

            return Task.WhenAll(topicTasks);
        }

        private class TopicComparer : EqualityComparer<TopicSpecification>
        {
            public override bool Equals(TopicSpecification x, TopicSpecification y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
            }

            public override int GetHashCode(TopicSpecification obj)
                => obj.Name.GetHashCode();
        }
    }
}

using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MyKafka.Consumers
{
    internal class KafkaConsumerFactory
    {
        private IConsumer<string, byte[]> _consumer;
        private MyConsumerConfiguration _consumerConfiguration;
        public KafkaConsumerFactory(MyConsumerConfiguration consumerConfiguration)
        {
            _consumerConfiguration = consumerConfiguration;
        }
        public IConsumer<string, byte[]>  Create()
        {
            if (_consumer != null)
            {
                return _consumer;
            }
            _consumer = new ConsumerBuilder<string, byte[]>(_consumerConfiguration.Configuration)                                
                .Build();
            _consumer.Subscribe(_consumerConfiguration.Topic.Name);
            return _consumer;
        }


    }
}

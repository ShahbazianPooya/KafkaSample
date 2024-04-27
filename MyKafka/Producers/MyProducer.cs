using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Kafka.Events;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace MyKafka.Producers
{
    public class MyProducer<T> where T : class, IEvent
    {
        private readonly IProducer<string, byte[]> _producer;
        private readonly MyProducerConfiguration _configuration;
        private readonly string _origin;
        private bool _schemaRegistered;
        public MyProducer(MyProducerConfiguration producerConfiguration)
        {
            if (producerConfiguration == default)
            {
                throw new ArgumentNullException(nameof(producerConfiguration));
            }
            _origin = Assembly.GetExecutingAssembly().FullName;
            _configuration = producerConfiguration;
            var producerFactory = new KafkaProducerFactory(producerConfiguration);
            _producer = producerFactory.Create();
        
        }
        public async Task CreateTopicsAsync()
        {
            var kafkaTopicFactory = new KafkaTopicFactory(_configuration);
            await kafkaTopicFactory.CreateTopicsAsync();
        }
        public async Task ProduceAsync(T evt, CancellationToken cancellationToken = default)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }
            var message = new Envelop<T>(evt, _configuration.Topic.Name, _origin);
         
            var kafkaMessage = new Message<string, byte[] >
            {
                Value = JsonSerializer.SerializeToUtf8Bytes(message),
                Key = evt.Key,
                Headers = new Headers()
            };
            kafkaMessage.Headers.Add(MyHeaders.EventTypeHeader, Encoding.UTF8.GetBytes(evt.GetType().FullName));
            await _producer.ProduceAsync(_configuration.Topic.Name, kafkaMessage, cancellationToken);
        }      
    }
}

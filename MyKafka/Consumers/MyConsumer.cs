using Confluent.Kafka;
using Kafka.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyKafka.Consumers
{
    public class MyConsumer
    {
        private IConsumer<string, byte[]> _consumer;
        Dictionary<string, List<dynamic>> _handlers;
        CancellationTokenSource _cancellationTokenSource;

        Dictionary<string, Type> _allEventsTypes;
        public MyConsumer(MyConsumerConfiguration myConsumerConfiguration, CancellationTokenSource cancellationTokenSource = default)
        {
            _cancellationTokenSource = cancellationTokenSource;
            var kafkaConsumerFactory = new KafkaConsumerFactory(myConsumerConfiguration);
            _consumer = kafkaConsumerFactory.Create();
            _allEventsTypes = new Dictionary<string, Type>();
            _handlers = new Dictionary<string, List<dynamic>>();
        }
        public void RegisterHandler<T>(IEventHandler<T> handler) where T : IEvent
        {
            var evtType = typeof(T);
            var eventTypeName = evtType.FullName;
            var eventTypeExists = _handlers.TryGetValue(eventTypeName, out List<dynamic> handlers);
            if (eventTypeExists)
            {
                handlers.Add(handler);
            }
            else
            {
                _handlers.Add(eventTypeName, new List<dynamic> { handler });
                _allEventsTypes.Add(eventTypeName, typeof(Envelop<T>));
            }

        }
        public async Task Consume()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var result = _consumer.Consume();
                if (!result.IsPartitionEOF && result != null)
                {
                    result.Message.Headers.TryGetLastBytes(MyHeaders.EventTypeHeader, out byte[] typeName);
                    var eventTypeName = Encoding.UTF8.GetString(typeName);
                    var eventTypeIsKnown = _allEventsTypes.TryGetValue(eventTypeName, out Type evtType);
                    if (eventTypeIsKnown)
                    {
                        var evt = await JsonSerializer.DeserializeAsync(new MemoryStream(result.Message.Value), evtType);
                        foreach (var handler in _handlers[eventTypeName])
                        {
                            await handler.HandleAsync((dynamic)evt);
                        }
                    }
                }
            }
        }
    }
}

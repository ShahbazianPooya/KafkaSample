using Kafka.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKafka.Consumers
{
    public interface IEventHandler<E> where E : IEvent
    {
        Task HandleAsync(Envelop<E> evt);
    }
}

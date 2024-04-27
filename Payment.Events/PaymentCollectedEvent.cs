using Kafka.Events;

namespace Payment.Events
{
    public class PaymentCollectedEvent : IEvent
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }

        public string Key => PaymentId.ToString();
        public override string ToString()
        {
            return $"PaymentId:{PaymentId} Amount:{Amount}";
        }
    }
}

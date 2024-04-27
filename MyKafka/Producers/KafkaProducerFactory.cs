using Confluent.Kafka;

namespace MyKafka.Producers
{
    internal class KafkaProducerFactory : IDisposable
    {
        private IProducer<string, byte[]> _producer;
        private readonly object _syncRoot = new();
        public MyProducerConfiguration Configuration { get; }

        public KafkaProducerFactory(MyProducerConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IProducer<string, byte[]> Create()
        {
            if (_producer != default)
            {
                return _producer;
            }

            lock (_syncRoot)
            {
                if (_producer != default)
                {
                    return _producer;
                }
                _producer = CreateProducer();
                return _producer;
            }
        }

        public void Dispose()
        {
            try
            {
                _producer?.Flush(TimeSpan.FromSeconds(10));
                _producer?.Dispose();
            }
            catch { }
        }

        private IProducer<string, byte[]> CreateProducer()
            => new ProducerBuilder<string, byte[]>(Configuration.Configuration)
                .SetErrorHandler((p, error) =>
                {
                    if (error.IsFatal)
                    {
                        try
                        {
                            p?.Dispose();
                        }
                        catch
                        {
                            // ignored
                        }
                        finally
                        {
                            _producer = null;
                        }
                    }
                }).Build();
    }
}

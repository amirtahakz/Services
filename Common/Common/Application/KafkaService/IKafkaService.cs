using Confluent.Kafka;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Services.Common.Application.KafkaService
{
    public interface IKafkaService
    {
        Task<bool> PublishEventAsync<TKeyType, TMessage>(string topic, TMessage message, ProducerConfig producerConfig);
        Task<TMessage> ConsumeEventAsync<TKeyType, TMessage>(string topic, ConsumerConfig consumerConfig, CancellationToken cancellationToken);
    }
}

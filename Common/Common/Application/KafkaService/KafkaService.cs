using Confluent.Kafka;
using Newtonsoft.Json;

namespace Common.Application.KafkaService
{
    public class KafkaService : IKafkaService
    {
        public async Task<bool> PublishEventAsync<TKeyType, TMessage>(string topic, TMessage message, ProducerConfig producerConfig)
        {
            try
            {
                using (var producer = new ProducerBuilder<TKeyType, string>(producerConfig).Build())
                {
                    var req = JsonConvert.SerializeObject(message);
                    var result = await producer.ProduceAsync(
                      topic,
                      new Message<TKeyType, string>
                      {
                          Value = req
                      });

                    return await Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }

            return await Task.FromResult(false);
        }

        public async Task<TMessage> ConsumeEventAsync<TKeyType, TMessage>(string topic, ConsumerConfig consumerConfig, CancellationToken cancellationToken)
        {

            try
            {
                using (var consumerBuilder = new ConsumerBuilder<TKeyType, string>(consumerConfig).Build())
                {
                    consumerBuilder.Subscribe(topic);
                    var cancelToken = new CancellationTokenSource();

                    try
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            var consumer = consumerBuilder.Consume(cancelToken.Token);
                            var res = JsonConvert.DeserializeObject<TMessage>(consumer.Message.Value);
                            return res;
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        consumerBuilder.Close();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }
            throw new Exception();
        }
    }
}

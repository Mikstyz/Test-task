using Confluent.Kafka;
using Serilog;
using System.Text.Json;
using Entities.Kafka;
using Entities.Product;

namespace Service.Kafka
{
    public class KafkaProducer
    {
        private static readonly ProducerConfig _config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
        };

        private const string Topic = "order-created";

        public static async Task<bool> SendOrderToKafka(int userId, int orderId, List<productsIds> productIds)
        {
            try
            {
                using var producer = new ProducerBuilder<Null, string>(_config).Build();

                var orderMessage = new OrderMessage
                {
                    UserId = userId,
                    OrderId = orderId, // Добавили orderId
                    ProductIds = productIds
                };

                var message = JsonSerializer.Serialize(orderMessage);

                var deliveryResult = await producer.ProduceAsync(Topic, new Message<Null, string> { Value = message });

                if (deliveryResult.Status != PersistenceStatus.Persisted)
                {
                    Log.Error($"Сообщение в {Topic} не улетело, статус: {deliveryResult.Status}");
                    return false;
                }

                Log.Information($"Сообщение в Kafka улетело в топик {Topic}. Order {orderId} для юзера {userId}, товары: {string.Join(", ", productIds.Select(p => $"{p.productId}:{p.Quantity}"))}");
                return true;
            }
            catch (KafkaException ex)
            {
                Log.Error($"Ошибка при работе с Kafka: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"Пиздец, не удалось отправить сообщение в Kafka: {ex.Message}");
                return false;
            }
        }
    }
}
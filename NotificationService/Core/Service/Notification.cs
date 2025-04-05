using Confluent.Kafka;
using System;
using System.Security.Cryptography.X509Certificates;

public static class KafkaConsumer
{
    public static void KafkaRead()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "test-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            // Подписываемся на топик
            consumer.Subscribe("order-created");

            Console.WriteLine("сообщения из order-created");

            try
            {
                while (true)
                {
                    // Читаем сообщение
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (consumeResult != null)
                    {
                        Console.WriteLine($"[Meesage] Order created: {consumeResult.Message.Value} " +
                                         $"[Partition: {consumeResult.Partition}, Offset: {consumeResult.Offset}]");
                    }
                }
            }

            catch (OperationCanceledException)
            {
                Console.WriteLine("Stop");
            }

            finally
            {
                consumer.Close();
            }
        }
    }
}
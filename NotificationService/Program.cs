using Confluent.Kafka;
using Serilog;

namespace Notification
{
    public static class Program
    {

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();


            //Read kafka message
            KafkaConsumer.KafkaRead();

        }
    }
}
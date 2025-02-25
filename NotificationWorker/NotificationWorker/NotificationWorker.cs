using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;

namespace NotificationWorker
{
   public class NotificationWorker
    {
        public async Task RunAsync()
        {
            Console.WriteLine("Worker basladi, mesajları dinleyir...");

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "notification_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Yeni bildiris alindi: {message}");
                await ProcessNotificationAsync(message);
            };

            channel.BasicConsume(queue: "notification_queue",
                                 autoAck: true,
                                 consumer: consumer);

            await Task.Delay(-1); 
        }

        private async Task ProcessNotificationAsync(string message)
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NotificationDB;Integrated Security=True;";

            using var sqlConnection = new SqlConnection(connectionString);
            await sqlConnection.OpenAsync();

            if (message.Contains("\"type\":\"email\""))
            {
                Console.WriteLine("Email gonderildi!");
            }
            else if (message.Contains("\"type\":\"sms\""))
            {
                var query = "INSERT INTO SmsLogs (Message) VALUES (@Message)";
                await sqlConnection.ExecuteAsync(query, new { Message = message });
                Console.WriteLine("Sms log bazaya yazıldı!");
            }

            var updateQuery = "UPDATE Notifications SET IsSent = 1 WHERE Message = @Message";
            await sqlConnection.ExecuteAsync(updateQuery, new { Message = message });

            Console.WriteLine("Notification  bazaya yazıldı!");
        }
    }
}

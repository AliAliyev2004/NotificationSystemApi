using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Text.Json;
using NotificationDomain.Entities;

namespace NotificationWorker
{
    public class NotificationWorker
    {
        public async Task RunAsync()
        {
            Console.WriteLine("Worker basladı mesajları dinleyir...");

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
                Console.WriteLine($"Yeni bildiris alındı: {message}");

                bool success = await ProcessNotificationAsync(message);

                if (success)
                {
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    Console.WriteLine("Bildiria islenerken sehv baş verdi.");
                }
            };

            channel.BasicConsume(queue: "notification_queue",
                                 autoAck: false,
                                 consumer: consumer);

            await Task.Delay(-1);
        }

        private async Task<bool> ProcessNotificationAsync(string message)
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NotificationDB;Integrated Security=True;";

            try
            {
                var notification = JsonSerializer.Deserialize<Notification>(message);
                if (notification == null)
                {
                    Console.WriteLine("Mesaj formatı sehvdir.");
                    return false;
                }

                using var sqlConnection = new SqlConnection(connectionString);
                await sqlConnection.OpenAsync();


                if (notification.Type == "email")
                {
                    Console.WriteLine("Email gonderildi!");
                }
                else if (notification.Type == "sms")
                {
                    var query = "INSERT INTO SmsLogs (Message) VALUES (@Message)";
                    await sqlConnection.ExecuteAsync(query, new { Message = notification.Message });
                    Console.WriteLine("SMS log bazaya yazıldı!");
                }

                // Notification-ın IsSent sahəsini yeniləyirik
                var updateQuery = "UPDATE Notifications SET IsSent = 1 WHERE Id = @Id";
                await sqlConnection.ExecuteAsync(updateQuery, new { Id = notification.Id });

                Console.WriteLine("Notification bazaya yazıldı!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xeta bas verdi: {ex.Message}");
                return false;
            }
        }
    }


}


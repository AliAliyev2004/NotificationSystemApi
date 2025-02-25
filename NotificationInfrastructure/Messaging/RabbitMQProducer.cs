using NotificationDomain.Entities;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace NotificationInfrastructure.Messaging
{
    public class RabbitMQProducer
    {
        private readonly IConnection _connection;
        private readonly string _queueName = "notification_queue";

        public RabbitMQProducer()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost", 
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
        }

        public void SendNotification(Notification notification)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonSerializer.Serialize(notification);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
            Console.WriteLine($" [x] Sent notification: {notification.Message}");
        }
    }
}

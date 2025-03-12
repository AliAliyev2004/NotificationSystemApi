using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using NotificationApplication.CQRS.Notifications.Commands.Requests;
using NotificationDomain.Entities;
using NotificationDomain.Interfaces;
using NotificationInfrastructure.Messaging;

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Notification>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly RabbitMQProducer _rabbitMQProducer;
    private readonly IDistributedCache _cache;

    public CreateNotificationCommandHandler(
        INotificationRepository notificationRepository,
        RabbitMQProducer rabbitMQProducer,
        IDistributedCache cache)
    {
        _notificationRepository = notificationRepository;
        _rabbitMQProducer = rabbitMQProducer;
        _cache = cache;
    }

    public async Task<Notification> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Message = request.Message,
            Type = request.Type,
            IsSent = false,
            CreatedAt = DateTime.UtcNow
        };
        var createdNotification = await _notificationRepository.AddAsync(notification);

        if (createdNotification != null)
        {
            _rabbitMQProducer.SendNotification(createdNotification);
            await _cache.RemoveAsync("AllNotifications");

            return createdNotification;
        }

        return null;
    }
}

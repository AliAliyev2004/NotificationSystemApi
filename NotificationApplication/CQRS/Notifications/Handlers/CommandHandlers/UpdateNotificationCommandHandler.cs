using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NotificationApplication.CQRS.Notifications.Commands.Requests;
using NotificationDomain.Interfaces;

namespace NotificationApplication.CQRS.Notifications.Handlers.CommandHandlers;

public class UpdateNotificationCommandHandler : IRequestHandler<UpdateNotificationCommand, bool>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDistributedCache _cache;

    public UpdateNotificationCommandHandler(INotificationRepository notificationRepository, IDistributedCache cache)
    {
        _notificationRepository = notificationRepository;
        _cache = cache;
    }

    public async Task<bool> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.Id);
        if (notification == null)
        {
            return false;
        }

        notification.Message = request.Message;
        notification.Type = request.Type;

        await _notificationRepository.UpdateAsync(notification);
        await _cache.RemoveAsync($"Notification:{notification.Id}");
        await _cache.RemoveAsync("AllNotifications");
        await _cache.SetStringAsync($"Notification:{notification.Id}", JsonConvert.SerializeObject(notification));

        return true;
    }
}


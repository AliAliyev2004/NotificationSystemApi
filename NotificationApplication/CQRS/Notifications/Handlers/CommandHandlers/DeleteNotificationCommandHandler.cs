using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using NotificationApplication.CQRS.Notifications.Commands.Requests;
using NotificationDomain.Interfaces;
using System.Collections.Specialized;

namespace NotificationApplication.CQRS.Notifications.Handlers.CommandHandlers;

public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, bool>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDistributedCache _cache;

    public DeleteNotificationCommandHandler(INotificationRepository notificationRepository, IDistributedCache cache)
    {
        _notificationRepository = notificationRepository;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.Id);
        if (notification == null)
        {
            return false;
        }

        await _notificationRepository.DeleteAsync(notification.Id);
        await _cache.RemoveAsync($"Notification:{request.Id}");
        await _cache.RemoveAsync("AllNotifications");

        return true;
    }
}

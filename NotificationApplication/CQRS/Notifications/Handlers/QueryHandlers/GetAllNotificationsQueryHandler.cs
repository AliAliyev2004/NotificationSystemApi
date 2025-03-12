using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NotificationApplication.CQRS.Notifications.Queries.Requests;
using NotificationDomain.Entities;
using NotificationDomain.Interfaces;

namespace NotificationApplication.CQRS.Notifications.Handlers.QueryHandlers;

public class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, IEnumerable<Notification>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDistributedCache _cache;

    public GetAllNotificationsQueryHandler(INotificationRepository notificationRepository, IDistributedCache cache)
    {
        _notificationRepository = notificationRepository;
        _cache = cache;
    }

    public async Task<IEnumerable<Notification>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notificationsJson = await _cache.GetStringAsync("AllNotifications");
        if (!string.IsNullOrEmpty(notificationsJson))
        {
            return JsonConvert.DeserializeObject<List<Notification>>(notificationsJson);
        }

        var notifications = await _notificationRepository.GetAllAsync();
        if (notifications.Any())
        {
            await _cache.SetStringAsync("AllNotifications", JsonConvert.SerializeObject(notifications));
        }

        return notifications;
    }
}

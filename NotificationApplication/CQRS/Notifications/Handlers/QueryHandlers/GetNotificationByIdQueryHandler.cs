using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NotificationApplication.CQRS.Notifications.Queries.Requests;
using NotificationApplication.CQRS.Notifications.Queries.Responses;
using NotificationDomain.Entities;
using NotificationDomain.Interfaces;

namespace NotificationApplication.CQRS.Notifications.Handlers.QueryHandlers;

public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, Notification>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDistributedCache _cache;

    public GetNotificationByIdQueryHandler(INotificationRepository notificationRepository, IDistributedCache cache)
    {
        _notificationRepository = notificationRepository;
        _cache = cache;
    }

    public async Task<Notification> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var notificationJson = await _cache.GetStringAsync($"Notification:{request.Id}");
        if (!string.IsNullOrEmpty(notificationJson))
        {
            return JsonConvert.DeserializeObject<Notification>(notificationJson);
        }

        var notification = await _notificationRepository.GetByIdAsync(request.Id);
        if (notification != null)
        {
            await _cache.SetStringAsync($"Notification:{request.Id}", JsonConvert.SerializeObject(notification));
        }

        return notification;
    }
}


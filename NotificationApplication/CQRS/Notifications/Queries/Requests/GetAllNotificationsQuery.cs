using MediatR;
using NotificationDomain.Entities;

namespace NotificationApplication.CQRS.Notifications.Queries.Requests;

public class GetAllNotificationsQuery : IRequest<IEnumerable<Notification>>
{
}

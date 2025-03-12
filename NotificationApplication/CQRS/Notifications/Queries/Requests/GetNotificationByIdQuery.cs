using MediatR;
using NotificationDomain.Entities;

namespace NotificationApplication.CQRS.Notifications.Queries.Requests;

public class GetNotificationByIdQuery : IRequest<Notification>
{
    public int Id { get; set; }

    public GetNotificationByIdQuery(int id)
    {
        Id = id;
    }
}

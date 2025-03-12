using MediatR;

namespace NotificationApplication.CQRS.Notifications.Commands.Requests;

public class DeleteNotificationCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteNotificationCommand(int id)
    {
        Id = id;
    }
}
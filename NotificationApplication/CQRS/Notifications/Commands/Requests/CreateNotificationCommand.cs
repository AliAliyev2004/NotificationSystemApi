using MediatR;
using NotificationDomain.Entities;

namespace NotificationApplication.CQRS.Notifications.Commands.Requests;
public class CreateNotificationCommand : IRequest<Notification>
{
    public string Message { get; set; }
    public string Type { get; set; }

    public CreateNotificationCommand(string message, string type)
    {
        Message = message;
        Type = type;
    }
}

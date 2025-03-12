using MediatR;


namespace NotificationApplication.CQRS.Notifications.Commands.Requests;

public class UpdateNotificationCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }

    public UpdateNotificationCommand(int id, string message, string type)
    {
        Id = id;
        Message = message;
        Type = type;
    }
}

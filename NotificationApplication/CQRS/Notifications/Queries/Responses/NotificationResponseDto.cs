namespace NotificationApplication.CQRS.Notifications.Queries.Responses;

public class NotificationResponseDto
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    public bool IsSent { get; set; }
    public DateTime CreatedAt { get; set; }
}

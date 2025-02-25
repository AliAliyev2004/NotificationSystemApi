namespace NotificationApplication.DTOs;

public class UpdateNotificationDto
{
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; 
    public bool IsSent { get; set; }
}
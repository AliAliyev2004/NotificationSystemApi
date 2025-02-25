namespace NotificationApplication.DTOs;


public class NotificationDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; 
    public bool IsSent { get; set; }
    public DateTime CreatedAt { get; set; }
}
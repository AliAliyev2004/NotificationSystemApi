using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotificationApplication.DTOs;
using NotificationApplication.Services;
using NotificationInfrastructure.Messaging;

namespace NotificationSystemApi1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _notificationService;
    private readonly RabbitMQProducer _rabbitMQProducer;

    public NotificationsController(NotificationService notificationService, RabbitMQProducer rabbitMQProducer)
    {
        _notificationService = notificationService;
        _rabbitMQProducer = rabbitMQProducer;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notifications = await _notificationService.GetAllAsync();
        return Ok(notifications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var notification = await _notificationService.GetByIdAsync(id);
        if (notification == null)
            return NotFound($"Notification with ID {id} not found.");

        return Ok(notification);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationDto createDto)
    {
        if (createDto == null)
            return BadRequest("Invalid data.");

        var notification = await _notificationService.CreateAsync(createDto);

       
        _rabbitMQProducer.SendNotification(new NotificationDomain.Entities.Notification
        {
            Id = notification.Id,
            Message = notification.Message,
            Type = notification.Type,
            IsSent = false,
            CreatedAt = notification.CreatedAt
        });

        return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNotificationDto updateDto)
    {
        var isUpdated = await _notificationService.UpdateAsync(id, updateDto);
        if (!isUpdated)
            return NotFound($"Notification with ID {id} not found.");

        return Ok("Notification updated successfully.");
    }

   
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var isDeleted = await _notificationService.DeleteAsync(id);
        if (!isDeleted)
            return NotFound($"Notification with ID {id} not found.");

        return Ok("Notification deleted successfully.");
    }
}

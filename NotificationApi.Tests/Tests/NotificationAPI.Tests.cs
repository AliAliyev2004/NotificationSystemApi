using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationApplication.CQRS.Notifications.Commands.Requests;
using NotificationApplication.CQRS.Notifications.Queries.Requests;
using NotificationApplication.DTOs;
using NotificationDomain.Entities;
using NotificationSystemApi.Controller;
using StackExchange.Redis;

public class NotificationsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<IDatabase> _mockRedis;
    private readonly NotificationsController _controller;
    public NotificationsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockRedis = new Mock<IDatabase>();
        _controller = new NotificationsController(_mockMediator.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult_WhenNotificationsExist()
    {
        
        var notifications = new List<Notification>
    {
        new Notification { Id = 1, Message = "Test", Type = "email" }
    };

        
        _mockMediator.Setup(m => m.Send(It.IsAny<GetAllNotificationsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(notifications);  

        
        var result = await _controller.GetAll();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(notifications);
    }
    [Fact]
    public async Task GetById_ShouldReturnOkResult_WhenNotificationExists()
    {
        var notification = new Notification { Id = 1, Message = "Test", Type = "email" };

        _mockMediator.Setup(m => m.Send(It.IsAny<GetNotificationByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        var result = await _controller.GetById(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(notification);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenNotificationIsCreated()
    {
        var createDto = new CreateNotificationDto { Message = "Test", Type = "email" };
        var createdNotification = new Notification
        {
            Id = 1,
            Message = "Test",
            Type = "email",
            IsSent = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateNotificationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdNotification); 

        var result = await _controller.Create(createDto);
        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult.StatusCode.Should().Be(201);
        createdResult.Value.Should().BeEquivalentTo("Notification created successfully.");
    }

    [Fact]
    public async Task Update_ShouldReturnOkResult_WhenNotificationIsUpdated()
    {
        var updateDto = new UpdateNotificationDto { Message = "Updated", Type = "sms" };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateNotificationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.Update(1, updateDto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be("Notification updated successfully.");
    }

    [Fact]
    public async Task Delete_ShouldReturnOkResult_WhenNotificationIsDeleted()
    {
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteNotificationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.Delete(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be("Notification deleted successfully.");
    }
}

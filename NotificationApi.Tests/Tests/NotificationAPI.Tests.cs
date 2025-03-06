using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationApplication.DTOs;
using NotificationApplication.Services;
using NotificationDomain.Entities;
using NotificationDomain.Interfaces;
using NotificationInfrastructure.Messaging;
using NotificationSystemApi.Controller;

public class NotificationsControllerTests
{
    private readonly Mock<INotificationRepository> _mockRepository;
    private readonly Mock<RabbitMQProducer> _mockRabbitMQProducer;
    private readonly Mock<IMapper> _mockMapper;
    private readonly NotificationsController _controller;

    public NotificationsControllerTests()
    {
        _mockRepository = new Mock<INotificationRepository>();
        _mockRabbitMQProducer = new Mock<RabbitMQProducer>();
        _mockMapper = new Mock<IMapper>();

        var notificationService = new NotificationService(_mockRepository.Object, _mockMapper.Object);
        _controller = new NotificationsController(notificationService, _mockRabbitMQProducer.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult_WhenNotificationsExist()
    {
        var notifications = new List<NotificationDto>
        {
            new NotificationDto { Id = 1, Message = "Test", Type = "email" }
        };

        _mockRepository.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Notification>
        {
            new Notification { Id = 1, Message = "Test", Type = "email", CreatedAt = DateTime.UtcNow }
        });

        _mockMapper.Setup(m => m.Map<IEnumerable<NotificationDto>>(It.IsAny<IEnumerable<Notification>>()))
                   .Returns(notifications);

        var result = await _controller.GetAll();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(notifications);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkResult_WhenNotificationExists()
    {
        var notificationDto = new NotificationDto { Id = 1, Message = "Test", Type = "email" };
        var notification = new NotificationDomain.Entities.Notification { Id = 1, Message = "Test", Type = "email", CreatedAt = DateTime.UtcNow };

        _mockRepository.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(notification);
        _mockMapper.Setup(m => m.Map<NotificationDto>(notification)).Returns(notificationDto);

        var result = await _controller.GetById(1);
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(notificationDto);
    }
    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenNotificationIsCreated()
    {
        var createDto = new CreateNotificationDto { Message = "Test", Type = "email" };
        var notification = new Notification { Id = 1, Message = "Test", Type = "email", CreatedAt = DateTime.UtcNow };
        var notificationDto = new NotificationDto { Id = 1, Message = "Test", Type = "email" };

        _mockMapper.Setup(m => m.Map<Notification>(createDto)).Returns(notification);
        _mockRepository.Setup(s => s.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<NotificationDto>(notification)).Returns(notificationDto);


        var result = await _controller.Create(createDto);


        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult.StatusCode.Should().Be(201);
        createdResult.Value.Should().BeEquivalentTo(notificationDto);
    }

    [Fact]
    public async Task Update_ShouldReturnOkResult_WhenNotificationIsUpdated()
    {

        var updateDto = new UpdateNotificationDto { Message = "Updated", Type = "sms" };
        var notification = new Notification { Id = 1, Message = "Test", Type = "email", CreatedAt = DateTime.UtcNow };

        _mockRepository.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(notification);
        _mockMapper.Setup(m => m.Map(updateDto, notification));

        _mockRepository.Setup(s => s.UpdateAsync(notification)).Returns(Task.CompletedTask);


        var result = await _controller.Update(1, updateDto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be("Notification updated successfully.");
    }

    [Fact]
    public async Task Delete_ShouldReturnOkResult_WhenNotificationIsDeleted()
    {
        var notification = new NotificationDomain.Entities.Notification { Id = 1, Message = "Test", Type = "email", CreatedAt = DateTime.UtcNow };

        _mockRepository.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(notification);
        _mockRepository.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(1);
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be("Notification deleted successfully.");
    }
}
using AutoMapper;
using NotificationApplication.DTOs;
using NotificationDomain.Entities;
using NotificationDomain.Interfaces;

namespace NotificationApplication.Services;
public class NotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> GetAllAsync()
    {
        var notifications = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<NotificationDto?> GetByIdAsync(int id)
    {
        var notification = await _repository.GetByIdAsync(id);
        return notification is null ? null : _mapper.Map<NotificationDto>(notification);
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
    {
        var notification = _mapper.Map<Notification>(dto);
        await _repository.AddAsync(notification);
        return _mapper.Map<NotificationDto>(notification);
    }

    public async Task<bool> UpdateAsync(int id, UpdateNotificationDto dto)
    {
        var notification = await _repository.GetByIdAsync(id);
        if (notification is null) return false;

        _mapper.Map(dto, notification);
        await _repository.UpdateAsync(notification);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var exists = await _repository.GetByIdAsync(id);
        if (exists is null) return false;

        await _repository.DeleteAsync(id);
        return true;
    }
}

using NotificationDomain.Entities;

namespace NotificationDomain.Interfaces;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetAllAsync();
    Task<Notification?> GetByIdAsync(int id);
    Task<Notification> AddAsync(Notification notification);
    Task<bool> UpdateAsync(Notification notification);
    Task<bool> DeleteAsync(int id);
}
using Microsoft.EntityFrameworkCore;
using NotificationDomain.Entities;
using NotificationDomain.Interfaces;
using NotificationInfrastructure.Data;

namespace NotificationInfrastructure.Repositories;
public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        return await _context.Notifications.ToListAsync();
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<Notification> AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
        return notification; 
    }


    public async Task<bool> UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        return await _context.SaveChangesAsync()>0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            return await _context.SaveChangesAsync() > 0;
        }
        return false;
    }
}
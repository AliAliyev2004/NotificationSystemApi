using Microsoft.EntityFrameworkCore;
using NotificationDomain.Entities;

namespace NotificationInfrastructure.Data;

 public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>().HasKey(n => n.Id);
        base.OnModelCreating(modelBuilder);
    }
}
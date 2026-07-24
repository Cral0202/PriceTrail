using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PriceTrail.Database;
using PriceTrail.Models.Notification;

namespace PriceTrail.Repositories.Notifications;

public class NotificationRepository
{
    public async Task<List<Notification>> GetNotificationsAsync()
    {
        using var db = new AppDbContext();

        return await db.Notifications
            .Include(n => n.Product)
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        using var db = new AppDbContext();

        db.Notifications.Add(notification);
        await db.SaveChangesAsync();
    }

    public async Task ClearNotificationsAsync()
    {
        using var db = new AppDbContext();

        db.Notifications.RemoveRange(db.Notifications);
        await db.SaveChangesAsync();
    }
}

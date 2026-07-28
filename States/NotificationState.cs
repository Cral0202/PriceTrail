using System.Collections.ObjectModel;
using System.Threading.Tasks;

using PriceTrail.Models.Notification;
using PriceTrail.Repositories.Notifications;

namespace PriceTrail.States;

public class NotificationState(NotificationRepository repo)
{
    public ObservableCollection<Notification> Notifications { get; } = [];

    public async Task InitializeAsync()
    {
        Notifications.Clear();

        foreach (var notification in await repo.GetNotificationsAsync())
        {
            Notifications.Add(notification);
        }
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        await repo.AddNotificationAsync(notification);
        Notifications.Insert(0, notification);
    }

    public async Task ClearAsync()
    {
        await repo.ClearNotificationsAsync();
        Notifications.Clear();
    }
}

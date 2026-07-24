using System.Collections.ObjectModel;
using System.Threading.Tasks;

using PriceTrail.Models.Notification;
using PriceTrail.Repositories.Notifications;

namespace PriceTrail.States;

public class NotificationState
{
    private readonly NotificationRepository _repo = new();

    public ObservableCollection<Notification> Notifications { get; } = [];

    public async Task InitializeAsync()
    {
        Notifications.Clear();

        foreach (var notification in await _repo.GetNotificationsAsync())
        {
            Notifications.Add(notification);
        }
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        await _repo.AddNotificationAsync(notification);
        Notifications.Insert(0, notification);
    }

    public async Task ClearAsync()
    {
        await _repo.ClearNotificationsAsync();
        Notifications.Clear();
    }
}

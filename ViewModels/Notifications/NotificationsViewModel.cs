using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Notification;
using PriceTrail.States;

namespace PriceTrail.ViewModels.Notifications;

public partial class NotificationsViewModel(NotificationState notificationState) : ViewModelBase
{
    public ObservableCollection<Notification> Notifications => notificationState.Notifications;

    [RelayCommand]
    private async Task ClearNotificationsAsync()
    {
        await notificationState.ClearAsync();
    }
}

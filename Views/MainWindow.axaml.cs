using Avalonia.Controls;
using Avalonia.Controls.Notifications;

using PriceTrail.Services;

namespace PriceTrail.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var manager = new WindowNotificationManager(this)
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 3
        };

        NotificationService.Instance.Initialize(manager);
    }
}

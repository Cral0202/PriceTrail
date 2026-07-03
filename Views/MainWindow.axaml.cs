using Avalonia.Controls;
using Avalonia.Controls.Notifications;

using PriceTrail.Services;

namespace PriceTrail.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Closing += MainWindow_Closing;

        var manager = new WindowNotificationManager(this)
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 3
        };

        ToastNotificationService.Instance.Initialize(manager);
    }

    private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
        ShowInTaskbar = false;
    }
}

using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;

using PriceTrail.Services;
using PriceTrail.ViewModels;

namespace PriceTrail.Views;

public partial class MainWindow : Window
{
    private bool _isShuttingDown; // Used to prevent infinite recursion loop

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
        if (_isShuttingDown)
            return;

        // Check whether to minimize to tray or close the application
        if (DataContext is MainWindowViewModel vm)
        {
            if (vm.MinimizeToTrayEnabled)
            {
                e.Cancel = true;
                Hide();
                ShowInTaskbar = false;
            }
            else
            {
                _isShuttingDown = true;

                if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown();
                }
            }
        }
    }
}

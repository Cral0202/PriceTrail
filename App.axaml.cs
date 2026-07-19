using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using PriceTrail.ViewModels;
using PriceTrail.Views;
using PriceTrail.Database;

using Microsoft.EntityFrameworkCore;

using System;
using Avalonia.Controls;
using PriceTrail.States;
using PriceTrail.Services;
using System.Threading.Tasks;

namespace PriceTrail;

public partial class App : Application
{
    private PlaywrightBrowserService? _playwrightBrowserService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Apply db migrations on startup
        using var db = new AppDbContext();
        db.Database.Migrate();

        _playwrightBrowserService = new PlaywrightBrowserService();

        // Open window
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };

            desktop.MainWindow = mainWindow;
            desktop.Exit += Desktop_Exit;

            _ = InitializeAppAsync(mainWindow);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task InitializeAppAsync(MainWindow mainWindow)
    {
        try
        {
            // Playwright
            await _playwrightBrowserService!.InitializeAsync();

            // State
            var appState = new AppState(_playwrightBrowserService);
            await appState.SettingsState.InitializeAsync();
            await appState.ProductState.LoadProductsAsync();

            mainWindow.DataContext = new MainWindowViewModel(appState);

            // Background services
            var priceRefreshService = new PriceRefreshService(appState.ProductState, appState.SettingsState);
            _ = priceRefreshService.RestartAsync();

            // Check for updates
            _ = appState.UpdateState.CheckForUpdatesAsync();
        }
        catch
        {
        }
    }

    private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        if (_playwrightBrowserService != null)
        {
            _ = _playwrightBrowserService.DisposeAsync();
            _playwrightBrowserService = null;
        }
    }

    private void TrayShowWindow_Click(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        desktop.MainWindow ??= new MainWindow();

        desktop.MainWindow.Show();
        desktop.MainWindow.ShowInTaskbar = true;
        desktop.MainWindow.WindowState = WindowState.Normal;
        desktop.MainWindow.Activate();
    }

    private void TrayExit_Click(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}

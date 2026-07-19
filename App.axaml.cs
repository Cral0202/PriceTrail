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

namespace PriceTrail;

public partial class App : Application
{
    private PlaywrightBrowserService? _playwrightBrowserService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        // Apply db migrations on startup
        using var db = new AppDbContext();
        db.Database.Migrate();

        // Playwright
        _playwrightBrowserService = new PlaywrightBrowserService();
        await _playwrightBrowserService.InitializeAsync();

        // State
        var appState = new AppState(_playwrightBrowserService); // TODO: Can we avoid having to pass the service all the way down to ProductState?
        await appState.SettingsState.InitializeAsync();
        await appState.ProductState.LoadProductsAsync();

        // Background services
        var priceRefreshService = new PriceRefreshService(appState.ProductState, appState.SettingsState);
        _ = priceRefreshService.RestartAsync();

        // Check for updates
        _ = appState.UpdateState.CheckForUpdatesAsync();

        // Open window
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(appState),
            };

            desktop.Exit += Desktop_Exit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ShutdownPlaywright()
    {
        // Force the async disposal to finish synchronously before the process dies
        _playwrightBrowserService?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        _playwrightBrowserService = null;
    }

    private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        ShutdownPlaywright();
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
            ShutdownPlaywright();
            desktop.Shutdown();
        }
    }
}

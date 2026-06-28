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
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        // Apply db migrations on startup
        using var db = new AppDbContext();
        db.Database.Migrate();

        // State
        var appState = new AppState();
        await appState.ProductState.LoadProductsAsync();

        // Background services
        var priceRefreshService = new PriceRefreshService(appState.ProductState);
        _ = priceRefreshService.StartAsync();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(appState),
            };
        }

        base.OnFrameworkInitializationCompleted();
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

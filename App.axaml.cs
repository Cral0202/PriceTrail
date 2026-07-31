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
using Microsoft.Extensions.DependencyInjection;
using PriceTrail.Repositories.Products;
using PriceTrail.ViewModels.Products;
using PriceTrail.ViewModels.Notifications;
using PriceTrail.ViewModels.Settings;
using PriceTrail.Services.Factories;
using PriceTrail.Repositories.Settings;
using PriceTrail.Repositories.Notifications;
using System.IO;

namespace PriceTrail;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        RegisterServices(services);
        Services = services.BuildServiceProvider();

        // Apply db migrations on startup
        var factory = Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using var db = factory.CreateDbContext();
        db.Database.Migrate();

        // Open window
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;
            desktop.Exit += Desktop_Exit;

            // Check if the application was launched on startup
            bool isAutostart = desktop.Args != null && desktop.Args.Contains("--autostart", StringComparer.OrdinalIgnoreCase);

            if (isAutostart)
            {
                mainWindow.WindowState = WindowState.Minimized;
                mainWindow.ShowInTaskbar = false;

                // Wait for the show, then hide it immediately
                void onWindowOpened(object? sender, EventArgs args)
                {
                    mainWindow.Hide();
                    mainWindow.Opened -= onWindowOpened;
                }

                mainWindow.Opened += onWindowOpened;
            }

            _ = InitializeAppAsync(mainWindow);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task InitializeAppAsync(MainWindow mainWindow)
    {
        var vm = (MainWindowViewModel)mainWindow.DataContext!;

        try
        {
            // Playwright
            var playwright = Services.GetRequiredService<PlaywrightBrowserService>();
            await playwright.InitializeAsync();

            // State
            var settingsState = Services.GetRequiredService<SettingsState>();
            var notificationState = Services.GetRequiredService<NotificationState>();
            var productState = Services.GetRequiredService<ProductState>();
            var updateState = Services.GetRequiredService<UpdateState>();

            await settingsState.InitializeAsync();
            await notificationState.InitializeAsync();
            await productState.LoadProductsAsync();

            // Launch on startup handling
            var startupService = Services.GetRequiredService<StartupService>();
            startupService.ApplyLaunchOnStartup();

            // Background services
            var priceRefreshService = Services.GetRequiredService<PriceRefreshService>();
            _ = priceRefreshService.RestartAsync();

            vm.IsLoading = false;

            // Check for updates
            _ = updateState.CheckForUpdatesAsync();
        }
        catch (Exception ex)
        {
            vm.ErrorMessage = ex.Message;
            vm.IsLoading = false;
        }
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Database
        services.AddDbContextFactory<AppDbContext>(options =>
        {
            Directory.CreateDirectory(AppPaths.Data);
            options.UseSqlite($"Data Source={AppPaths.Database}");
        });

        // Repositories
        services.AddSingleton<ProductRepository>();
        services.AddSingleton<ProductPageRepository>();
        services.AddSingleton<PriceHistoryRepository>();
        services.AddSingleton<SettingsRepository>();
        services.AddSingleton<NotificationRepository>();

        // States
        services.AddSingleton<SettingsState>();
        services.AddSingleton<NotificationState>();
        services.AddSingleton<UpdateState>();
        services.AddSingleton<ProductState>();

        // Services
        services.AddSingleton<PlaywrightBrowserService>();
        services.AddSingleton<ProductExtractorService>();
        services.AddSingleton<PriceNotificationService>();
        services.AddSingleton<NativeNotificationService>();
        services.AddSingleton<PriceRefreshService>();
        services.AddSingleton<NavigationService>();
        services.AddSingleton<UpdateService>();
        services.AddSingleton<ToastNotificationService>();
        services.AddSingleton<StartupService>();

        // Views
        services.AddTransient<MainWindow>();

        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ProductsViewModel>();
        services.AddTransient<NotificationsViewModel>();
        services.AddTransient<SettingsViewModel>();

        services.AddTransient<AddProductModalViewModel>();

        // Factories
        services.AddSingleton<ProductDetailsViewModelFactory>();
        services.AddSingleton<EditProductModalViewModelFactory>();
        services.AddSingleton<AddProductPageModalViewModelFactory>();
        services.AddSingleton<OverviewViewModelFactory>();
        services.AddSingleton<HistoryViewModelFactory>();
    }

    private async void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        if (Services is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
    }

    private void TrayShowWindow_Click(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        desktop.MainWindow ??= Services.GetRequiredService<MainWindow>();

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

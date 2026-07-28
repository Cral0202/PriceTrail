using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Services;
using PriceTrail.States;
using PriceTrail.ViewModels.Notifications;
using PriceTrail.ViewModels.Products;
using PriceTrail.ViewModels.Settings;

namespace PriceTrail.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly SettingsState _settingsState;

    public NavigationService Navigation { get; }

    // Used for the sidebar
    public enum NavigationPage
    {
        Products,
        Notifications,
        Settings
    }

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProductsActive))]
    [NotifyPropertyChangedFor(nameof(IsNotificationsActive))]
    [NotifyPropertyChangedFor(nameof(IsSettingsActive))]

    private NavigationPage _currentPage = NavigationPage.Products;

    public ProductsViewModel? ProductsViewModel { get; }
    public NotificationsViewModel? NotificationsViewModel { get; }
    public SettingsViewModel? SettingsViewModel { get; }

    public bool IsProductsActive => CurrentPage == NavigationPage.Products;
    public bool IsNotificationsActive => CurrentPage == NavigationPage.Notifications;
    public bool IsSettingsActive => CurrentPage == NavigationPage.Settings;

    public bool MinimizeToTrayEnabled => _settingsState.Settings.MinimizeToTrayEnabled;

    public MainWindowViewModel(NavigationService navigation, SettingsState settingsState, ProductsViewModel productsViewModel, NotificationsViewModel notificationsViewModel, SettingsViewModel settingsViewModel)
    {
        _settingsState = settingsState;

        ProductsViewModel = productsViewModel;
        NotificationsViewModel = notificationsViewModel;
        SettingsViewModel = settingsViewModel;

        Navigation = navigation;

        Navigation.NavigateAndClearHistory(ProductsViewModel);
    }

    [RelayCommand]
    private void ShowProducts()
    {
        Navigation.NavigateAndClearHistory(ProductsViewModel!);
        CurrentPage = NavigationPage.Products;
    }

    [RelayCommand]
    private void ShowNotifications()
    {
        Navigation.NavigateAndClearHistory(NotificationsViewModel!);
        CurrentPage = NavigationPage.Notifications;
    }

    [RelayCommand]
    private void ShowSettings()
    {
        Navigation.NavigateAndClearHistory(SettingsViewModel!);
        CurrentPage = NavigationPage.Settings;
    }
}

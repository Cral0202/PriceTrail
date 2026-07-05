using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.States;

namespace PriceTrail.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // Used for the sidebar
    public enum NavigationPage
    {
        Products,
        Settings
    }

    [ObservableProperty]
    private object _currentViewModel;

    [ObservableProperty]
    public partial ObservableObject? CurrentModalViewModel { get; set; }

    public bool IsModalOpen => CurrentModalViewModel != null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProductsActive))]
    [NotifyPropertyChangedFor(nameof(IsSettingsActive))]
    private NavigationPage _currentPage = NavigationPage.Products;

    public ProductsViewModel ProductsViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }

    public bool IsProductsActive => CurrentPage == NavigationPage.Products;
    public bool IsSettingsActive => CurrentPage == NavigationPage.Settings;

    public MainWindowViewModel(AppState appState)
    {
        ProductsViewModel = new ProductsViewModel(this, appState.ProductState);
        SettingsViewModel = new SettingsViewModel(appState.SettingsState);

        CurrentViewModel = ProductsViewModel;
    }

    [RelayCommand]
    private void ShowProducts()
    {
        CurrentViewModel = ProductsViewModel;
        CurrentPage = NavigationPage.Products;
    }

    [RelayCommand]
    private void ShowSettings()
    {
        CurrentViewModel = SettingsViewModel;
        CurrentPage = NavigationPage.Settings;
    }

    partial void OnCurrentModalViewModelChanged(ObservableObject? value)
    {
        OnPropertyChanged(nameof(IsModalOpen));
    }
}

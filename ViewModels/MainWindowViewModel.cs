using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.States;

namespace PriceTrail.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial object CurrentViewModel { get; set; }

    public ProductsViewModel ProductsViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }

    public MainWindowViewModel(AppState appState)
    {
        ProductsViewModel = new ProductsViewModel(this, appState);
        SettingsViewModel = new SettingsViewModel();

        CurrentViewModel = ProductsViewModel;
    }

    [RelayCommand]
    private void ShowProducts()
    {
        CurrentViewModel = ProductsViewModel;
    }

    [RelayCommand]
    private void ShowSettings()
    {
        CurrentViewModel = SettingsViewModel;
    }
}

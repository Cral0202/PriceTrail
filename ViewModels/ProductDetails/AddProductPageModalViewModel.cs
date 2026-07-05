using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.Services;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class AddProductPageModalViewModel(MainWindowViewModel mainWindow, ProductState productState, Product product) : ViewModelBase
{
    [ObservableProperty]
    public partial string Url { get; set; } = "";

    [RelayCommand]
    private async Task AddProductPageAsync()
    {
        string? errorMessage = await productState.AddProductPageToProductAsync(product, Url);
        Close();

        if (errorMessage != null)
        {
            ToastNotificationService.Instance.ShowMessage("Failed to add URL", errorMessage, Avalonia.Controls.Notifications.NotificationType.Error);
        }
    }

    [RelayCommand]
    private void Close()
    {
        mainWindow.CurrentModalViewModel = null;
    }
}

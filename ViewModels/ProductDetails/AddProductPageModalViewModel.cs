using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.Services;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class AddProductPageModalViewModel(NavigationService navigation, ProductState productState, Product product, ToastNotificationService toastService) : ViewModelBase
{
    private CancellationTokenSource? _cts;

    [ObservableProperty]
    public partial string Url { get; set; } = "";

    [RelayCommand]
    private async Task AddProductPageAsync()
    {
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        string? errorMessage = await productState.AddProductPageToProductAsync(product, Url, _cts.Token);
        Close();

        if (errorMessage != null)
        {
            toastService.ShowMessage("Failed to add URL", errorMessage, Avalonia.Controls.Notifications.NotificationType.Error);
        }
    }

    [RelayCommand]
    private void Close()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        navigation.CloseModal();
    }
}

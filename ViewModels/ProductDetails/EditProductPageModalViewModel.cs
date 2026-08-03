using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.Services;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class EditProductPageModalViewModel(NavigationService navigation, ProductState productState, Product product, ProductPage productPage) : ViewModelBase
{
    [ObservableProperty]
    public partial string NewStoreName { get; set; } = productPage.StoreName;

    [RelayCommand]
    private async Task ChangeStoreNameAsync()
    {
        var trimmedName = NewStoreName.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
            return;

        if (productPage.StoreName == trimmedName)
            return;

        productPage.StoreName = trimmedName;
        await productState.UpdateProductPageAsync(productPage);
    }

    [RelayCommand]
    private async Task DeleteProductPageAsync()
    {
        await productState.DeleteProductPageAsync(product, productPage);
        await Close();
    }

    [RelayCommand]
    private async Task Close()
    {
        // Check if store name should be updated
        if (productPage.StoreName != NewStoreName)
        {
            await ChangeStoreNameAsync();
        }

        navigation.CloseModal();
    }
}

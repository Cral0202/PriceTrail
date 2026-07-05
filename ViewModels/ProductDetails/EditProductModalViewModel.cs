using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class EditProductModalViewModel(MainWindowViewModel mainWindow, ProductState productState, Product product) : ViewModelBase
{
    [ObservableProperty]
    public partial string NewProductName { get; set; } = product.Name;

    [RelayCommand]
    private async Task ChangeProductNameAsync()
    {
        var trimmedName = NewProductName.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
            return;

        if (product.Name == trimmedName)
            return;

        product.Name = trimmedName;
        await productState.UpdateProductAsync(product);
    }

    [RelayCommand]
    private async Task DeleteProductAsync()
    {
        await productState.DeleteProductAsync(product);
        await Close();
        mainWindow.CurrentViewModel = mainWindow.ProductsViewModel;
    }

    [RelayCommand]
    private async Task Close()
    {
        // Check if product name should be updated
        if (product.Name != NewProductName)
        {
            await ChangeProductNameAsync();
        }

        mainWindow.CurrentModalViewModel = null;
    }
}

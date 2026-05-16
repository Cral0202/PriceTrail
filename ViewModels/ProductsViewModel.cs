using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models;
using PriceTrail.Services;
using PriceTrail.States;

namespace PriceTrail.ViewModels;

public partial class ProductsViewModel(MainWindowViewModel mainWindow, AppState appState) : ObservableObject
{
    private readonly ProductState _productState = appState.ProductState;

    public ObservableCollection<Product> Products => _productState.Products;

    [ObservableProperty]
    public partial bool IsAddProductModalOpen { get; set; }

    [ObservableProperty]
    public partial string NewProductName { get; set; } = "";

    [RelayCommand]
    private async Task AddProductAsync()
    {
        var trimmedName = NewProductName.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
            return;

        var product = new Product
        {
            Name = trimmedName
        };

        await _productState.AddProductAsync(product);

        NewProductName = "";
        IsAddProductModalOpen = false;
    }

    [RelayCommand]
    private void OpenProduct(Product product)
    {
        mainWindow.CurrentViewModel = new ProductDetailsViewModel(mainWindow, _productState, product);
    }

    /**********/
    /* MODALS */
    /**********/

    [RelayCommand]
    private void OpenAddProductModal()
    {
        IsAddProductModalOpen = true;
    }

    [RelayCommand]
    private void CloseAddProductModal()
    {
        NewProductName = "";
        IsAddProductModalOpen = false;
    }
}

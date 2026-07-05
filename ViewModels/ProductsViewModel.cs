using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.States;
using PriceTrail.ViewModels.ProductDetails;

namespace PriceTrail.ViewModels;

public partial class ProductsViewModel(MainWindowViewModel mainWindow, ProductState productState) : ViewModelBase
{
    public ObservableCollection<Product> Products => productState.Products;

    [RelayCommand]
    private void OpenProduct(Product product)
    {
        mainWindow.CurrentViewModel = new ProductDetailsViewModel(mainWindow, productState, product);
    }

    /**********/
    /* MODALS */
    /**********/

    [RelayCommand]
    private void OpenAddProductModal()
    {
        mainWindow.CurrentModalViewModel = new AddProductModalViewModel(mainWindow, productState);
    }
}

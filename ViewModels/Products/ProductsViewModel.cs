using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.Services;
using PriceTrail.Services.Factories;
using PriceTrail.States;

namespace PriceTrail.ViewModels.Products;

public partial class ProductsViewModel(NavigationService navigation, ProductDetailsViewModelFactory productDetailsFactory, AddProductModalViewModel addProductModalViewModel, ProductState productState) : ViewModelBase
{
    public ObservableCollection<Product> Products => productState.Products;

    [RelayCommand]
    private void OpenProduct(Product product)
    {
        navigation.NavigateTo(productDetailsFactory.Create(product));
    }

    /**********/
    /* MODALS */
    /**********/

    [RelayCommand]
    private void OpenAddProductModal()
    {
        navigation.OpenModal(addProductModalViewModel);
    }
}

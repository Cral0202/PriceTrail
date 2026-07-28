using PriceTrail.Models.Product;
using PriceTrail.States;
using PriceTrail.ViewModels.ProductDetails;

namespace PriceTrail.Services.Factories;

public class AddProductPageModalViewModelFactory(NavigationService navigation, ProductState productState, ToastNotificationService toastService)
{
    public AddProductPageModalViewModel Create(Product product)
    {
        return new AddProductPageModalViewModel(navigation, productState, product, toastService);
    }
}

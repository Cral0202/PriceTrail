using PriceTrail.Models.Product;
using PriceTrail.ViewModels.ProductDetails;

namespace PriceTrail.Services.Factories;

public class HistoryViewModelFactory
{
    public HistoryViewModel Create(Product product)
    {
        return new HistoryViewModel(product);
    }
}

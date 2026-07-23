using System.Threading.Tasks;

using PriceTrail.Models.Product;

namespace PriceTrail.Services;

public class PriceNotificationService
{
    private readonly NativeNotificationService _notificationService = new();

    public async Task CheckForNotifications(Product product, ProductPage existingPage, ProductPage updatedPage)
    {
        {
            await CheckPriceDrop(product, updatedPage);
        }
    }

    // Checks whether the fetched price is lower than the current lowest product price
    private async Task CheckPriceDrop(Product product, ProductPage updatedPage)
    {
        if (updatedPage.Price is decimal newPrice && product.LowestPrice is decimal currentLowestPrice && newPrice < currentLowestPrice)
        {
            await _notificationService.SendNotificationAsync("Price dropped!", $"{product.Name}\n{currentLowestPrice} → {newPrice} {updatedPage.Currency}");
        }
    }
}

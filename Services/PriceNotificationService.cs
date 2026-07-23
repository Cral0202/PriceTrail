using System.Threading.Tasks;

using PriceTrail.Models.Product;

namespace PriceTrail.Services;

public class PriceNotificationService
{
    private readonly NativeNotificationService _notificationService = new();

    public async Task CheckForNotifications(Product product, ProductPage existingPage, ProductPage updatedPage)
    {
        {
            await CheckPriceDrop(product, existingPage, updatedPage);
        }
    }

    private async Task CheckPriceDrop(Product product, ProductPage existingPage, ProductPage updatedPage)
    {
        if (existingPage.Price is decimal previous && updatedPage.Price is decimal current && current < previous)
        {
            await _notificationService.SendNotificationAsync("Price dropped!", $"{product.Name}\n{previous} → {current} {updatedPage.Currency}");
        }
    }
}

using System.Threading.Tasks;

using PriceTrail.Models.Notification;
using PriceTrail.Models.Product;
using PriceTrail.States;

namespace PriceTrail.Services;

public class PriceNotificationService(NotificationState notificationState)
{
    private readonly NativeNotificationService _notificationService = new();

    public async Task CheckForNotifications(Product product, ProductPage previousPage, ProductPage newPage)
    {
        {
            await CheckPriceDrop(product, newPage);
        }
    }

    // Checks whether the fetched price is lower than the current lowest product price
    private async Task CheckPriceDrop(Product product, ProductPage updatedPage)
    {
        if (updatedPage.Price is decimal newPrice && product.LowestPrice is decimal currentLowestPrice && newPrice < currentLowestPrice)
        {
            var notification = new Notification
            {
                ProductId = product.Id,
                Title = "Price dropped!",
                Message = $"{product.Name}\n{currentLowestPrice} → {newPrice} {updatedPage.Currency}",
                Type = NotificationType.PriceDrop
            };

            await notificationState.AddNotificationAsync(notification);
            await _notificationService.SendNotificationAsync(notification.Title, notification.Message);
        }
    }
}

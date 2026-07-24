using System;

namespace PriceTrail.Models.Notification;

public class Notification
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string Title { get; set; } = "";

    public string Message { get; set; } = "";

    public NotificationType Type { get; set; }

    public int? ProductId { get; set; }
    public Product.Product? Product { get; set; }
}

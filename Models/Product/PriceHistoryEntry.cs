using System;

namespace PriceTrail.Models.Product;

public class PriceHistoryEntry
{
    public int Id { get; set; }
    public int ProductPageId { get; set; } // Foreign key
    public ProductPage? ProductPage { get; set; } // Navigation property

    public decimal Price { get; set; }
    public string Currency { get; set; } = "";
    public DateTime Timestamp { get; set; }
}

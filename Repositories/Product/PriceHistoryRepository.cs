using System;
using System.Threading.Tasks;

using PriceTrail.Database;
using PriceTrail.Models.Product;

namespace PriceTrail.Repositories.Product;

public class PriceHistoryRepository
{
    public async Task<PriceHistoryEntry> AddPriceHistoryEntryAsync(ProductPage page)
    {
        using var db = new AppDbContext();

        var entry = new PriceHistoryEntry
        {
            ProductPageId = page.Id,
            Price = page.Price,
            Currency = page.Currency,
            Timestamp = DateTime.UtcNow
        };

        db.PriceHistoryEntries.Add(entry);
        await db.SaveChangesAsync();
        return entry;
    }
}

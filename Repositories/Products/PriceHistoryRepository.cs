using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PriceTrail.Database;
using PriceTrail.Models.Product;

namespace PriceTrail.Repositories.Products;

public class PriceHistoryRepository(IDbContextFactory<AppDbContext> contextFactory)
{
    public async Task<PriceHistoryEntry> AddPriceHistoryEntryAsync(ProductPage page)
    {
        using var db = await contextFactory.CreateDbContextAsync();

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

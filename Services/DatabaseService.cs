using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PriceTrail.Database;
using PriceTrail.Models;

namespace PriceTrail.Services;

public class DatabaseService
{
    public async Task<List<Product>> GetProductsAsync()
    {
        using var db = new AppDbContext();

        return await db.Products.Include(p => p.ProductPages).ThenInclude(pp => pp.PriceHistory).ToListAsync();
    }

    public async Task AddProductAsync(Product product)
    {
        using var db = new AppDbContext();

        db.Products.Add(product);
        await db.SaveChangesAsync();
    }

    public async Task AddProductPageAsync(
        Product product,
        ProductPage page)
    {
        using var db = new AppDbContext();

        page.ProductId = product.Id;

        db.ProductPages.Add(page);
        await db.SaveChangesAsync();
    }

    public async Task UpdateProductPageAsync(ProductPage page)
    {
        using var db = new AppDbContext();

        db.ProductPages.Update(page);
        await db.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product product)
    {
        using var db = new AppDbContext();

        db.Products.Remove(product);
        await db.SaveChangesAsync();
    }

    public async Task DeleteProductPageAsync(ProductPage page)
    {
        using var db = new AppDbContext();

        db.ProductPages.Remove(page);
        await db.SaveChangesAsync();
    }

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

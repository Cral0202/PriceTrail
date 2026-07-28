using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PriceTrail.Database;

using PriceTrail.Models.Product;

namespace PriceTrail.Repositories.Products;

public class ProductRepository(IDbContextFactory<AppDbContext> contextFactory)
{
    public async Task<List<Product>> GetProductsAsync()
    {
        using var db = await contextFactory.CreateDbContextAsync();

        return await db.Products.Include(p => p.ProductPages).ThenInclude(pp => pp.PriceHistory).ToListAsync();
    }

    public async Task AddProductAsync(Product product)
    {
        using var db = await contextFactory.CreateDbContextAsync();

        db.Products.Add(product);
        await db.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        using var db = await contextFactory.CreateDbContextAsync();

        db.Products.Update(product);
        await db.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product product)
    {
        using var db = await contextFactory.CreateDbContextAsync();

        db.Products.Remove(product);
        await db.SaveChangesAsync();
    }
}

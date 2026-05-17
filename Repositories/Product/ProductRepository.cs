using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PriceTrail.Database;

using ProductModel = PriceTrail.Models.Product.Product;

namespace PriceTrail.Repositories.Product;

public class ProductRepository
{
    public async Task<List<ProductModel>> GetProductsAsync()
    {
        using var db = new AppDbContext();

        return await db.Products.Include(p => p.ProductPages).ThenInclude(pp => pp.PriceHistory).ToListAsync();
    }

    public async Task AddProductAsync(ProductModel product)
    {
        using var db = new AppDbContext();

        db.Products.Add(product);
        await db.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(ProductModel product)
    {
        using var db = new AppDbContext();

        db.Products.Update(product);
        await db.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(ProductModel product)
    {
        using var db = new AppDbContext();

        db.Products.Remove(product);
        await db.SaveChangesAsync();
    }
}

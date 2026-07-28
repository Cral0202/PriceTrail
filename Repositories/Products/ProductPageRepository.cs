using System.Threading.Tasks;

using PriceTrail.Database;
using ProductModel = PriceTrail.Models.Product.Product;
using PriceTrail.Models.Product;
using Microsoft.EntityFrameworkCore;

namespace PriceTrail.Repositories.Products;

public class ProductPageRepository(IDbContextFactory<AppDbContext> contextFactory)
{
    public async Task AddProductPageAsync(ProductModel product, ProductPage page)
    {
        using var db = await contextFactory.CreateDbContextAsync();

        page.ProductId = product.Id;

        db.ProductPages.Add(page);
        await db.SaveChangesAsync();
    }

    public async Task UpdateProductPageAsync(ProductPage page)
    {
        using var db = await contextFactory.CreateDbContextAsync();

        db.ProductPages.Update(page);
        await db.SaveChangesAsync();
    }

    public async Task DeleteProductPageAsync(ProductPage page)
    {
        using var db = await contextFactory.CreateDbContextAsync();

        db.ProductPages.Remove(page);
        await db.SaveChangesAsync();
    }
}

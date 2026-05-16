using System.Threading.Tasks;

using PriceTrail.Database;
using PriceTrail.Models;

namespace PriceTrail.Repositories;

public class ProductPageRepository
{
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

    public async Task DeleteProductPageAsync(ProductPage page)
    {
        using var db = new AppDbContext();

        db.ProductPages.Remove(page);
        await db.SaveChangesAsync();
    }
}

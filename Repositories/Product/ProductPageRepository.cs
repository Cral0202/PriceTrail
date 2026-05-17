using System.Threading.Tasks;

using PriceTrail.Database;
using ProductModel = PriceTrail.Models.Product.Product;
using PriceTrail.Models.Product;

namespace PriceTrail.Repositories.Product;

public class ProductPageRepository
{
    public async Task AddProductPageAsync(ProductModel product, ProductPage page)
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

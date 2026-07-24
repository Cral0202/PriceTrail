using System.IO;

using Microsoft.EntityFrameworkCore;

using PriceTrail.Models.Notification;
using PriceTrail.Models.Product;
using PriceTrail.Models.Settings;

namespace PriceTrail.Database;

public class AppDbContext : DbContext
{
    public DbSet<AppSettings> Settings => Set<AppSettings>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductPage> ProductPages => Set<ProductPage>();
    public DbSet<PriceHistoryEntry> PriceHistoryEntries => Set<PriceHistoryEntry>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Directory.CreateDirectory(AppPaths.Data);
        optionsBuilder.UseSqlite($"Data Source={AppPaths.Database}");
    }
}

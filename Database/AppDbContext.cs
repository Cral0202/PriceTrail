using System;
using System.IO;

using Microsoft.EntityFrameworkCore;

using PriceTrail.Models.Product;
using PriceTrail.Models.Settings;

namespace PriceTrail.Database;

public class AppDbContext : DbContext
{
    public DbSet<AppSettings> Settings => Set<AppSettings>();

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductPage> ProductPages => Set<ProductPage>();
    public DbSet<PriceHistoryEntry> PriceHistoryEntries => Set<PriceHistoryEntry>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dataDirectory = Path.Combine(
            AppContext.BaseDirectory,
            "Data");

        Directory.CreateDirectory(dataDirectory);

        var databasePath = Path.Combine(
            dataDirectory,
            "pricetrail.db");

        optionsBuilder.UseSqlite(
            $"Data Source={databasePath}");
    }
}

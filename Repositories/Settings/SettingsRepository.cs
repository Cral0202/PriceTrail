using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PriceTrail.Database;
using PriceTrail.Models.Settings;

namespace PriceTrail.Repositories.Settings;

public class SettingsRepository(IDbContextFactory<AppDbContext> contextFactory)
{
    public async Task<AppSettings> LoadAsync()
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        var settings = await db.Settings.FirstOrDefaultAsync();

        if (settings != null)
            return settings;

        settings = new AppSettings();

        db.Settings.Add(settings);
        await db.SaveChangesAsync();

        return settings;
    }

    public async Task SaveAsync(AppSettings settings)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        db.Settings.Update(settings);
        await db.SaveChangesAsync();
    }
}

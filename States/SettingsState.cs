using System.ComponentModel;
using System.Threading.Tasks;

using PriceTrail.Models.Settings;
using PriceTrail.Repositories.Settings;

namespace PriceTrail.States;

public class SettingsState(SettingsRepository repo)
{
    public AppSettings Settings { get; private set; } = new();

    private async void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        await repo.SaveAsync(Settings);
    }

    public async Task InitializeAsync()
    {
        Settings = await repo.LoadAsync();
        Settings.PropertyChanged += OnSettingsChanged;
    }
}

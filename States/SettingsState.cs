using System.ComponentModel;
using System.Threading.Tasks;

using PriceTrail.Models.Settings;
using PriceTrail.Repositories.Settings;

namespace PriceTrail.States;

public class SettingsState
{
    private readonly SettingsRepository _repo = new();

    public AppSettings Settings { get; private set; } = new();

    private async void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        await _repo.SaveAsync(Settings);
    }

    public async Task InitializeAsync()
    {
        Settings = await _repo.LoadAsync();
        Settings.PropertyChanged += OnSettingsChanged;
    }
}

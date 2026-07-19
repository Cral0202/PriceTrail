using PriceTrail.Services;

namespace PriceTrail.States;

public class AppState(PlaywrightBrowserService playrightBrowserService)
{
    public SettingsState SettingsState { get; } = new();
    public ProductState ProductState { get; } = new(playrightBrowserService);
    public UpdateState UpdateState { get; } = new();
}

using PriceTrail.Services;

namespace PriceTrail.States;

public class AppState
{
    public SettingsState SettingsState { get; }
    public NotificationState NotificationState { get; }
    public UpdateState UpdateState { get; }
    public ProductState ProductState { get; }

    public AppState(PlaywrightBrowserService playrightBrowserService)
    {
        SettingsState = new SettingsState();
        NotificationState = new NotificationState();
        UpdateState = new UpdateState();
        ProductState = new ProductState(playrightBrowserService, NotificationState); // TODO: Not clean
    }
}

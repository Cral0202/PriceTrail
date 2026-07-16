namespace PriceTrail.States;

public class AppState
{
    public SettingsState SettingsState { get; } = new();
    public ProductState ProductState { get; } = new();
    public UpdateState UpdateState { get; } = new();
}

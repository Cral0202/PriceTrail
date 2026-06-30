/*********************************************************************************/
/* Handles periodically refreshing all tracked product prices in the background. */
/*********************************************************************************/

using System;
using System.Threading;
using System.Threading.Tasks;

using PriceTrail.States;
using PriceTrail.Models.Settings;
using System.ComponentModel;

namespace PriceTrail.Services;

public class PriceRefreshService
{
    private readonly ProductState _productState;
    private readonly SettingsState _settingsState;

    private CancellationTokenSource? _cts;
    private Task? _runningTask;

    public PriceRefreshService(ProductState productState, SettingsState settingsState)
    {
        _productState = productState;
        _settingsState = settingsState;

        _settingsState.Settings.PropertyChanged += OnSettingsChanged;
    }

    private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppSettings.AutomaticPriceRefreshEnabled) or nameof(AppSettings.PriceRefreshInterval))
        {
            _ = RestartAsync();
        }
    }

    public async Task RestartAsync()
    {
        _cts?.Cancel();

        if (_runningTask != null)
            await _runningTask;

        _cts = new CancellationTokenSource();

        _runningTask = RunAsync(_cts.Token);
    }

    private async Task RunAsync(CancellationToken token)
    {
        var settings = _settingsState.Settings;

        if (!settings.AutomaticPriceRefreshEnabled)
            return;

        using var timer = new PeriodicTimer(settings.PriceRefreshInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(token))
            {
                await _productState.RefreshAllProductPricesAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when loop is canceled
        }
    }
}

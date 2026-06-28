using System;
using System.Threading;
using System.Threading.Tasks;

using PriceTrail.States;

namespace PriceTrail.Services;

public class PriceRefreshService(ProductState productState)
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(5)); // TODO: Should be setting instead of hardcoded 5 minutes

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        while (await _timer.WaitForNextTickAsync(cancellationToken))
        {
            await productState.RefreshAllProductPricesAsync();
        }
    }
}

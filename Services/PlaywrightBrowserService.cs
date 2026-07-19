using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Playwright;

namespace PriceTrail.Services;

public class PlaywrightBrowserService : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public async Task InitializeAsync()
    {
        // Use custom directory
        Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", AppPaths.Playwright);
        Directory.CreateDirectory(AppPaths.Playwright);

        _playwright = await Playwright.CreateAsync();

        await EnsureChromiumInstalledAsync();

        _browser = await _playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = true
            });
    }

    public IBrowser Browser => _browser ?? throw new InvalidOperationException("Browser has not been initialized.");

    private static Task EnsureChromiumInstalledAsync()
    {
        return Task.Run(() =>
        {
            var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);

            if (exitCode != 0)
                throw new InvalidOperationException($"Playwright Chromium installation failed with exit code {exitCode}.");
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }

        _playwright?.Dispose();
        _playwright = null;

        GC.SuppressFinalize(this);
    }
}

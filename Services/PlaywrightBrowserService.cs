using System;
using System.IO;
using System.Linq;
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

        if (OperatingSystem.IsMacOS())
        {
            // In the .app bundle, the .playwright folder lives in "Contents/Resources" instead of next to the executable
            var resourcesDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "Resources"));
            Environment.SetEnvironmentVariable("PLAYWRIGHT_DRIVER_SEARCH_PATH", resourcesDir);
        }

        _playwright = await Playwright.CreateAsync();

        await EnsureChromiumInstalledAsync();
        DeleteOldBrowsers(); // Old browser folders will remain on Playwright updates, so we delete them

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

    private void DeleteOldBrowsers()
    {
        try
        {
            var baseDir = new DirectoryInfo(AppPaths.Playwright);

            if (!baseDir.Exists)
                return;

            var browserDirs = baseDir.GetDirectories("chromium-*");

            if (browserDirs.Length <= 1)
                return;

            // Sort folders by last modified time (newest first)
            var sortedDirs = browserDirs.OrderByDescending(d => d.LastWriteTimeUtc).ToList();
            var currentActiveDir = sortedDirs.First();

            foreach (var oldDir in sortedDirs.Skip(1))
            {
                try
                {
                    oldDir.Delete(recursive: true);
                }
                catch (IOException)
                {
                }
            }
        }
        catch (Exception)
        {
        }
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

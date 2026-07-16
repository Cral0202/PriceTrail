using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PriceTrail.Services;

public class UpdateService
{
    private static readonly HttpClient _httpClient = new();

    static UpdateService()
    {
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PriceTrail");
    }

    public async Task<UpdateCheckResult> CheckForUpdatesAsync()
    {
        try
        {
            var json = await _httpClient.GetStringAsync(Constants.LatestReleaseApi);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var latestTag = root.GetProperty("tag_name").GetString()?.TrimStart('v');

            if (string.IsNullOrWhiteSpace(latestTag))
                return UpdateCheckResult.NoUpdate();

            if (!Version.TryParse(Constants.AppVersion, out var current))
                return UpdateCheckResult.NoUpdate();

            if (!Version.TryParse(latestTag, out var latest))
                return UpdateCheckResult.NoUpdate();

            return latest > current
                ? UpdateCheckResult.UpdateAvailable(
                    latestTag,
                    root.GetProperty("html_url").GetString()!)
                : UpdateCheckResult.NoUpdate();
        }
        catch
        {
            // Ignore failures
            return UpdateCheckResult.NoUpdate();
        }
    }
}

public class UpdateCheckResult
{
    public bool IsUpdateAvailable { get; init; }

    public string? LatestVersion { get; init; }

    public string? ReleaseUrl { get; init; }

    public static UpdateCheckResult NoUpdate() => new();

    public static UpdateCheckResult UpdateAvailable(string version, string url) =>
        new()
        {
            IsUpdateAvailable = true,
            LatestVersion = version,
            ReleaseUrl = url
        };
}

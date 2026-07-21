using HtmlAgilityPack;

using Microsoft.Playwright;

using PriceTrail.Models.Product;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PriceTrail.Services;

public class ProductExtractorService(PlaywrightBrowserService browserService)
{
    private const float NavigationTimeoutMs = 30000;

    private readonly SemaphoreSlim _maxURLSem = new(5); // Max URLs that can be fetched concurrently

    public async Task<ExtractionResult> ExtractAsync(string url, CancellationToken cancellationToken = default)
    {
        await _maxURLSem.WaitAsync(cancellationToken);

        try
        {
            return await ExtractInternalAsync(url, cancellationToken);
        }
        finally
        {
            _maxURLSem.Release();
        }
    }

    private async Task<ExtractionResult> ExtractInternalAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var baseUri))
            {
                return ExtractionResult.Failure("The provided URL is invalid.");
            }

            await using var context =
                await browserService.Browser.NewContextAsync(
                    new BrowserNewContextOptions
                    {
                        Locale = "en-US",
                        UserAgent =
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                            "AppleWebKit/537.36 (KHTML, like Gecko) " +
                            "Chrome/124.0.0.0 Safari/537.36"
                    });

            var page = await context.NewPageAsync();

            // Cancellation immediately aborts any active Playwright network requests
            await using var registration = cancellationToken.Register(() =>
            {
                _ = page.CloseAsync();
            });

            await page.GotoAsync(
                url,
                new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = NavigationTimeoutMs
                });

            var html = await page.ContentAsync();

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var scriptNodes = document.DocumentNode.SelectNodes("//script[@type='application/ld+json']");

            if (scriptNodes == null)
                return ExtractionResult.Failure("No JSON-LD metadata found on the page.");

            foreach (var script in scriptNodes)
            {
                try
                {
                    using var doc = JsonDocument.Parse(script.InnerText);

                    foreach (var node in GetNodes(doc.RootElement))
                    {
                        if (!IsProduct(node))
                            continue;

                        if (!node.TryGetProperty("offers", out var offersElement))
                            continue;

                        // Offers can be object or array
                        var offers = offersElement.ValueKind == JsonValueKind.Array ? offersElement[0] : offersElement;

                        var priceString = TryResolvePrice(offers);

                        if (string.IsNullOrEmpty(priceString) || !decimal.TryParse(priceString, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
                            continue;

                        var currency = TryResolveCurrency(offers);
                        var imageUrl = TryResolveImage(node, baseUri);
                        var storeName = "Unknown Store";

                        if (offers.TryGetProperty("seller", out var seller))
                        {
                            storeName = seller.ValueKind == JsonValueKind.Object
                                ? seller.TryGetProperty("name", out var sellerName) ? sellerName.GetString() ?? storeName
                                : storeName : seller.GetString() ?? storeName;
                        }

                        return ExtractionResult.Success(
                            new ProductPage
                            {
                                Url = url,
                                StoreName = storeName,
                                Price = price,
                                Currency = currency ?? "Unknown Currency",
                                ImageUrl = imageUrl
                            });
                    }
                }
                catch (JsonException)
                {
                    // Invalid JSON-LD blocks
                }
            }

            return ExtractionResult.Failure("Analyzed metadata blocks, but none contained valid product schema.");
        }
        catch (OperationCanceledException)
        {
            return ExtractionResult.Failure("The page extraction was cancelled.");
        }
        catch (PlaywrightException) when (cancellationToken.IsCancellationRequested)
        {
            // Playwright throws a TargetClosedException when page.CloseAsync() breaks an active GotoAsync call
            return ExtractionResult.Failure("The page extraction was cancelled.");
        }
        catch (TimeoutException)
        {
            return ExtractionResult.Failure("The page took too long to load.");
        }
        catch (PlaywrightException e)
        {
            return ExtractionResult.Failure($"Could not load the web page. ({e.Message})");
        }
        catch (Exception e)
        {
            return ExtractionResult.Failure($"An unexpected error occurred: {e.Message}");
        }
    }

    private static IEnumerable<JsonElement> GetNodes(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in root.EnumerateArray())
            {
                foreach (var node in GetNodes(item))
                    yield return node;
            }
        }
        else if (root.ValueKind == JsonValueKind.Object)
        {
            yield return root;

            if (root.TryGetProperty("@graph", out var graph) && graph.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in graph.EnumerateArray())
                {
                    foreach (var node in GetNodes(item))
                        yield return node;
                }
            }
        }
    }

    private static bool IsProduct(JsonElement element)
    {
        if (!element.TryGetProperty("@type", out var typeProperty))
            return false;

        if (typeProperty.ValueKind == JsonValueKind.String)
            return typeProperty.GetString() == "Product";

        if (typeProperty.ValueKind == JsonValueKind.Array)
        {
            foreach (var type in typeProperty.EnumerateArray())
            {
                if (type.ValueKind == JsonValueKind.String && type.GetString() == "Product")
                    return true;
            }
        }

        return false;
    }

    private static string? TryResolvePrice(JsonElement element)
    {
        // Direct flat price
        if (element.TryGetProperty("price", out var p) && p.ValueKind != JsonValueKind.Null)
            return p.ToString();

        // Price range - use lowest price
        if (element.TryGetProperty("lowPrice", out var lp))
            return lp.ToString();

        // Nested price specification
        if (element.TryGetProperty("priceSpecification", out var spec))
        {
            var targetSpec = spec.ValueKind == JsonValueKind.Array && spec.GetArrayLength() > 0 ? spec[0] : spec;

            if (targetSpec.ValueKind == JsonValueKind.Object && targetSpec.TryGetProperty("price", out var specPrice))
                return specPrice.ToString();
        }

        return null;
    }

    private static string? TryResolveCurrency(JsonElement element)
    {
        // Try root level first
        if (element.TryGetProperty("priceCurrency", out var c))
            return c.GetString();

        // Try priceSpecification
        if (element.TryGetProperty("priceSpecification", out var spec))
        {
            var targetSpec = spec.ValueKind == JsonValueKind.Array && spec.GetArrayLength() > 0 ? spec[0] : spec;

            if (targetSpec.ValueKind == JsonValueKind.Object && targetSpec.TryGetProperty("priceCurrency", out var specCurrency))
                return specCurrency.GetString();
        }

        return "Unknown Currency";
    }

    private static string? TryResolveImage(JsonElement element, Uri baseUri)
    {
        if (!element.TryGetProperty("image", out var imageElement) || imageElement.ValueKind == JsonValueKind.Null)
            return null;

        // Direct string URL
        if (imageElement.ValueKind == JsonValueKind.String)
            return ResolveUrl(imageElement.GetString(), baseUri);

        // Array (could be strings or ImageObjects)
        if (imageElement.ValueKind == JsonValueKind.Array && imageElement.GetArrayLength() > 0)
        {
            var firstImage = imageElement[0];

            if (firstImage.ValueKind == JsonValueKind.String)
            {
                return ResolveUrl(firstImage.GetString(), baseUri);
            }

            if (firstImage.ValueKind == JsonValueKind.Object && firstImage.TryGetProperty("url", out var urlProperty))
            {
                return ResolveUrl(urlProperty.GetString(), baseUri);
            }
        }

        // Single nested ImageObject
        if (imageElement.ValueKind == JsonValueKind.Object && imageElement.TryGetProperty("url", out var urlElement))
            return ResolveUrl(urlElement.GetString(), baseUri);

        return null;
    }

    private static string? ResolveUrl(string? url, Uri baseUri)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri))
            return absoluteUri.ToString();

        if (Uri.TryCreate(baseUri, url, out var resolvedUri))
            return resolvedUri.ToString();

        return null;
    }
}

using HtmlAgilityPack;

using PriceTrail.Models.Product;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace PriceTrail.Services;

public class ProductExtractorService
{
    private readonly HttpClient _httpClient;

    public ProductExtractorService()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression =
            DecompressionMethods.GZip |
            DecompressionMethods.Deflate |
            DecompressionMethods.Brotli
        };

        _httpClient = new HttpClient(handler);

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/124.0.0.0 Safari/537.36");

        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("text/html"));

        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");

        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<ProductPage?> ExtractAsync(string url)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(url);
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var scriptNodes = document.DocumentNode.SelectNodes("//script[@type='application/ld+json']");

            if (scriptNodes == null)
                return null;

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

                        var priceString = offers.GetProperty("price").ToString();
                        var currency = offers.GetProperty("priceCurrency").GetString();

                        if (!decimal.TryParse(priceString, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
                            continue;

                        var storeName = "Unknown Store";

                        if (offers.TryGetProperty("seller", out var seller))
                        {
                            storeName = seller.ValueKind == JsonValueKind.Object
                                ? seller.GetProperty("name").GetString() ?? storeName
                                : seller.GetString() ?? storeName;
                        }

                        return new ProductPage
                        {
                            Url = url,
                            StoreName = storeName,
                            Price = price,
                            Currency = currency ?? "Unknown Currency"
                        };
                    }
                }
                catch
                {
                    // Invalid JSON-LD blocks
                }
            }
        }
        catch
        {
            // Network errors
        }

        return null;
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

            if (root.TryGetProperty("@graph", out var graph))
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
                if (type.GetString() == "Product")
                    return true;
            }
        }

        return false;
    }
}

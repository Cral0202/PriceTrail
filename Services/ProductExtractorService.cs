using HtmlAgilityPack;

using PriceTrail.Models;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace PriceTrail.Services;

public class ProductExtractorService
{
    private readonly HttpClient _httpClient = new();

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
    }

    public async Task<ProductInfo?> ExtractAsync(string url)
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
                var json = script.InnerText;

                using var doc = JsonDocument.Parse(json);

                var root = doc.RootElement;

                if (root.TryGetProperty("@type", out var typeProperty))
                {
                    var type = typeProperty.GetString();

                    if (type == "Product")
                    {
                        if (root.TryGetProperty("offers", out var offers))
                        {
                            var price = offers.GetProperty("price").ToString();

                            var currency = offers.GetProperty("priceCurrency").GetString();

                            return new ProductInfo
                            {
                                Price = price,
                                Currency = currency
                            };
                        }
                    }
                }
            }
            catch
            {
                // Ignore invalid JSON-LD blocks
            }
        }

        return null;
    }
}

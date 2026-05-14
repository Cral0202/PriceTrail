using HtmlAgilityPack;

using PriceTrail.Models;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PriceTrail.Services;

public class ProductExtractorService
{
    private readonly HttpClient _httpClient = new();

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
                            var price = offers.GetProperty("price").GetString();

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

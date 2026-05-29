namespace PriceTrail.Models.Product;

public class ExtractionResult
{
    public bool IsSuccess { get; init; }
    public ProductPage? Page { get; init; }
    public string? ErrorMessage { get; init; }

    public static ExtractionResult Success(ProductPage page) => new() { IsSuccess = true, Page = page };
    public static ExtractionResult Failure(string message) => new() { IsSuccess = false, ErrorMessage = message };
}

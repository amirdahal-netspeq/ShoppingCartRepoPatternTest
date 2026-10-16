namespace ShoppingCart.Application.Products
{
    public sealed record ProductDto(
        Guid Id,
        string Name,
        string? Description,
        decimal Price,
        string Currency,
        int StockQuantity,
        bool IsActive);
}

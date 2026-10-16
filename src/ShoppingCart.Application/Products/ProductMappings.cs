using ShoppingCart.Domain.Products;

namespace ShoppingCart.Application.Products
{
    public static class ProductMappings
    {
        public static ProductDto ToDto(this Product p) =>
            new(p.Id, p.Name, p.Description, p.Price.Amount, p.Price.Currency, p.StockQuantity, p.IsActive);
    }
}

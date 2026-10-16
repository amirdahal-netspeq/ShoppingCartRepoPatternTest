using ShoppingCart.Domain.Products;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.Products
{
    public sealed record CreateProductRequest
    {
        [Required, StringLength(Product.MaxNameLength)]
        public string Name { get; init; } = default!;

        [StringLength(2000)]
        public string? Description { get; init; }

        [Range(0.01, 1_000_000)]
        public decimal Price { get; init; }

        [Required, StringLength(3, MinimumLength = 3)]
        public string Currency { get; init; } = "INR";

        [Range(0, 1_000_000)]
        public int StockQuantity { get; init; }
    }

    public sealed record UpdateProductRequest
    {
        [Required, StringLength(Product.MaxNameLength)]
        public string Name { get; init; } = default!;

        [StringLength(2000)]
        public string? Description { get; init; }

        [Range(0.01, 1_000_000)]
        public decimal Price { get; init; }
    }
}

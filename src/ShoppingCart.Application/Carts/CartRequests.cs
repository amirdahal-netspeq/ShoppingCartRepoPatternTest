using ShoppingCart.Domain.Carts;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Application.Carts
{
    public sealed record AddCartItemRequest
    {
        public Guid ProductId { get; init; }
        [Range(1, Cart.MaxQuantityPerItem)]
        public int Quantity { get; init; }
    }

    public sealed record ChangeCartItemQuantityRequest
    {
        [Range(1, Cart.MaxQuantityPerItem)]
        public int Quantity { get; init; }
    }
}

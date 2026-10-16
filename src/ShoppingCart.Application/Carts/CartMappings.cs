using ShoppingCart.Domain.Carts;

namespace ShoppingCart.Application.Carts
{
    public static class CartMappings
    {
        public static CartDto ToDto(this Cart cart) => new CartDto(
            cart.CustomerId,
            cart.Currency,
            cart.Items.Select(i => new CartItemDto(i.ProductId, i.ProductName, i.UnitPrice.Amount, i.Quantity, i.LineTotal.Amount)).ToList(),
            cart.Total.Amount);

        public static CartDto EmptyCart(Guid customerId, string currency)
            => new(customerId, currency, Array.Empty<CartItemDto>(), 0m);
    }
}

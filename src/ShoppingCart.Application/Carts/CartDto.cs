namespace ShoppingCart.Application.Carts
{
    public sealed record CartItemDto(
        Guid ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity,
        decimal LineTotal
    );

    public sealed record CartDto(
        Guid CustomerId,
        string Currency,
        IReadOnlyCollection<CartItemDto> Items,
        decimal Total
    );
}

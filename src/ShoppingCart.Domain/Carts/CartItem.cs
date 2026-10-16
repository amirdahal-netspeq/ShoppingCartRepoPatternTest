using ShoppingCart.Domain.Common;

namespace ShoppingCart.Domain.Carts
{
    public sealed class CartItem : Entity
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = default!;     // snapshot at the time of adding
        public Money UnitPrice { get; private set; } = default!;        // snapshot at the time of adding
        public int Quantity { get; private set; }
        public Money LineTotal => UnitPrice * Quantity;

        private CartItem() { }                                          // only for EF Core

        internal CartItem(Guid productId, string productName, Money unitPrice, int quantity)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        internal void ChangeQuantity(int quantity) => Quantity = quantity;
    }
}

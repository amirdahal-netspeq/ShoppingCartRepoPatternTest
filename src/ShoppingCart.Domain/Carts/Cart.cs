using ShoppingCart.Domain.Common;

namespace ShoppingCart.Domain.Carts
{
    public sealed class Cart : Entity, IAggregateRoot
    {
        public const int MaxQuantityPerItem = 10;

        private readonly List<CartItem> _items = new();

        public Guid CustomerId { get; private set; }
        public string Currency { get; private set; } = default!;
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();
        public bool IsEmpty => _items.Count == 0;
        public Money Total => _items.Aggregate(Money.Zero(Currency), (sum, item) => sum + item.LineTotal);

        private Cart() { }                                              // only for EF Core

        public Cart(Guid customerId, string currency)
        {
            if (customerId == Guid.Empty) throw new DomainException("A customer is required.");

            Id = Guid.NewGuid();
            CustomerId = customerId;
            Currency = new Money(0, currency).Currency;                 // reuses Money's validation and normalization
        }

        public bool Contains(Guid productId) => _items.Any(i => i.ProductId == productId);

        public void AddItem(Guid productId, string productName, Money unitPrice, int quantity)
        {
            if (productId == Guid.Empty) throw new DomainException("A product is required.");
            if (string.IsNullOrWhiteSpace(productName)) throw new DomainException("Product name is required.");
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
            if (unitPrice.Currency != Currency)
                throw new DomainException($"This cart uses {Currency}, but the product is priced in {unitPrice.Currency}.");

            var existing = _items.FirstOrDefault(i => i.ProductId == productId);
            var newQuantity = (existing?.Quantity ?? 0) + quantity;
            EnsureWithinLimit(newQuantity);

            if (existing is null) _items.Add(new CartItem(productId, productName, unitPrice, quantity));
            else existing.ChangeQuantity(newQuantity);                  // same product merges into one line
        }

        public void ChangeItemQuantity(Guid productId, int quantity)
        {
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero. Remove the item instead.");
            EnsureWithinLimit(quantity);
            FindItem(productId).ChangeQuantity(quantity);
        }

        public void RemoveItem(Guid productId) => _items.Remove(FindItem(productId));

        public void Clear() => _items.Clear();

        private CartItem FindItem(Guid productId)
            => _items.FirstOrDefault(i => i.ProductId == productId)
               ?? throw new DomainException("That product is not in the cart.");

        private static void EnsureWithinLimit(int quantity)
        {
            if (quantity > MaxQuantityPerItem)
                throw new DomainException($"You can buy at most {MaxQuantityPerItem} of the same product.");
        }
    }
}

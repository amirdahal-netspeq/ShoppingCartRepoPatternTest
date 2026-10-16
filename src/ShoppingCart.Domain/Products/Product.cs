using ShoppingCart.Domain.Common;

namespace ShoppingCart.Domain.Products
{
    public sealed class Product : Entity, IAggregateRoot
    {
        public const int MaxNameLength = 200;

        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }
        public Money Price { get; private set; } = default!;
        public int StockQuantity { get; private set; }
        public bool IsActive { get; private set; }

        private Product() { }

        public Product(string name, string? description, Money price, int stockQuantity)
        {
            if (stockQuantity < 0)
                throw new DomainException("Stock quantity cannot be negative.");

            Id = Guid.NewGuid();

            UpdateDetails(name, description);
            ChangePrice(price);
            StockQuantity = stockQuantity;
            IsActive = true;
        }

        public void UpdateDetails(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Product name is required.");
            if (name.Trim().Length > MaxNameLength)
                throw new DomainException($"Product name cannot exceed {MaxNameLength} characters.");

            Name = name.Trim();
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        public void ChangePrice(Money price)
        {
            ArgumentNullException.ThrowIfNull(price);
            if (price.Amount <= 0) throw new DomainException("Price must be greater than zero.");
            Price = price;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0) throw new DomainException("Quantity to add must be greater than zero.");
            StockQuantity += quantity;
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0) throw new DomainException("Quantity to remove must be greater than zero.");
            if (quantity > StockQuantity) throw new DomainException($"Insufficient stock for '{Name}'.");
            StockQuantity -= quantity;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public bool CanBePurchased(int quantity) => IsActive && quantity > 0 && StockQuantity >= quantity;
    }
}

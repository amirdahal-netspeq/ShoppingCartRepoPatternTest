using ShoppingCart.Application.Products;
using ShoppingCart.Domain.Products;

namespace ShoppingCart.Application.Tests.Fakes
{
    public sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Store { get; } = new();

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(Store.FirstOrDefault(p => p.Id == id));

        public Task AddAsync(Product product, CancellationToken ct = default)
        {
            Store.Add(product);
            return Task.CompletedTask;
        }

        public void Remove(Product product)
            => Store.Remove(product);

        public Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken ct = default)
            => Task.FromResult(Store.Any(p =>
            string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase) && p.Id != excludeId));
    }
}

using ShoppingCart.Application.Carts;
using ShoppingCart.Domain.Carts;

namespace ShoppingCart.Application.Tests.Fakes
{
    public sealed class FakeCartRepository : ICartRepository
    {
        public List<Cart> Store { get; } = new();


        public Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
            => Task.FromResult(Store.FirstOrDefault(c => c.CustomerId == customerId));

        public Task<Cart?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(Store.FirstOrDefault(c => c.Id == id));

        public Task AddAsync(Cart cart, CancellationToken ct = default)
        {
            Store.Add(cart);
            return Task.CompletedTask;
        }

        public void Remove(Cart cart)
            => Store.Remove(cart);
    }
}

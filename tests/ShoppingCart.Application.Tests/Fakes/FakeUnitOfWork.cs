using ShoppingCart.Application.Abstractions;

namespace ShoppingCart.Application.Tests.Fakes
{
    public sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCalls { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            SaveCalls++;
            return Task.FromResult(1);
        }
    }
}

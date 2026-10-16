using ShoppingCart.Domain.Common;

namespace ShoppingCart.Application.Abstractions
{
    public interface IRepository<T> where T : Entity, IAggregateRoot
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        void Remove(T entity);
    }
}

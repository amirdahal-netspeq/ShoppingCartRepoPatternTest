using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Abstractions;
using ShoppingCart.Domain.Common;

namespace ShoppingCart.Infrastructure.Persistence.Repositories
{
    internal abstract class Repository<T> : IRepository<T> where T : Entity, IAggregateRoot
    {
        protected readonly AppDbContext Db;

        protected Repository(AppDbContext db)
            => Db = db;


        public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Db.Set<T>().FirstOrDefaultAsync(e => e.Id == id, ct);

        public async Task AddAsync(T entity, CancellationToken ct = default)
            => await Db.Set<T>().AddAsync(entity, ct);

        public void Remove(T entity)
            => Db.Set<T>().Remove(entity);
    }
}

using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Carts;
using ShoppingCart.Domain.Carts;

namespace ShoppingCart.Infrastructure.Persistence.Repositories
{
    internal sealed class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext db) : base(db) { }

        // An aggregate is always loaded whole: a cart without its items would be a broken cart.
        public override Task<Cart?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id, ct);

        public Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
            => Db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.CustomerId == customerId, ct);
    }
}

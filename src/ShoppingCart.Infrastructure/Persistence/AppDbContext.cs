using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Abstractions;
using ShoppingCart.Application.Common;
using ShoppingCart.Domain.Carts;
using ShoppingCart.Domain.Products;

namespace ShoppingCart.Infrastructure.Persistence
{
    public sealed class AppDbContext : DbContext, IUnitOfWork
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cart> Carts => Set<Cart>();
        // There is no DbSet<CartItem>: items are reached only through their aggregate root.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            try
            {
                return await base.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DataConflictException("The data was changed by someone else.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new DataConflictException("The change violates a data constraint.", ex);
            }
        }
    }
}

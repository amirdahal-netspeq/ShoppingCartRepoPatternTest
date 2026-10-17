using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Products;
using ShoppingCart.Domain.Products;

namespace ShoppingCart.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext db) : base(db) { }

        public Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken ct = default)
        {
            var normalized = name.Trim().ToLower();
            return Db.Products.AnyAsync(p => p.Name.ToLower() == normalized && (excludeId == null || p.Id != excludeId), ct);
        }
    }
}

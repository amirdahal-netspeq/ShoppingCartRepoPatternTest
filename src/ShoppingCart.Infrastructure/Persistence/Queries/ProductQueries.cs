using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Common;
using ShoppingCart.Application.Products;

namespace ShoppingCart.Infrastructure.Persistence.Queries
{
    internal sealed class ProductQueries : IProductQueries
    {
        private readonly AppDbContext _db;

        public ProductQueries(AppDbContext db)
            => _db = db;

        public Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.Products.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => p.ToDto())
            .FirstOrDefaultAsync(ct);

        public async Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
        {
            var query = _db.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term));
            }

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => p.ToDto())
                .ToListAsync(ct);

            return new PagedResult<ProductDto>(items, page, pageSize, total);
        }
    }
}

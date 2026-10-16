using ShoppingCart.Application.Common;

namespace ShoppingCart.Application.Products
{
    public interface IProductQueries
    {
        Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    }
}

using ShoppingCart.Application.Common;

namespace ShoppingCart.Application.Products
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
        Task<Result<ProductDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
        Task<Result<ProductDto>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
        Task<Result> DeactivateAsync(Guid id, CancellationToken ct = default);
    }
}

using ShoppingCart.Application.Abstractions;
using ShoppingCart.Application.Common;
using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Products;

namespace ShoppingCart.Application.Products
{
    public sealed class ProductService : IProductService
    {
        public const int MaxPageSize = 100;

        private readonly IProductRepository _products;
        private readonly IProductQueries _queries;
        private readonly IUnitOfWork _uow;

        public ProductService(IProductRepository products, IProductQueries queries, IUnitOfWork uow)
            => (_products, _queries, _uow) = (products, queries, uow);

        public Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
            var term = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
            return _queries.GetPagedAsync(page, pageSize, term, ct);
        }

        public async Task<Result<ProductDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var product = await _queries.GetByIdAsync(id, ct);
            return product is null
                ? Error.NotFound("Product not found.")
                : Result.Success(product);
        }

        public async Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
        {
            if (await _products.NameExistsAsync(request.Name, excludeId: null, ct))
                return Error.Conflict("Product name already exists.");

            var product = new Product(
                request.Name,
                request.Description,
                new Money(request.Price, request.Currency),
                request.StockQuantity);

            await _products.AddAsync(product, ct);
            await _uow.SaveChangesAsync(ct);

            return Result.Success(product.ToDto());
        }

        public async Task<Result<ProductDto>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
        {
            var product = await _products.GetByIdAsync(id, ct);
            if (product is null)
                return Error.NotFound("Product not found.");

            if (await _products.NameExistsAsync(request.Name, excludeId: id, ct))
                return Error.Conflict("A product with this name already exists.");

            product.UpdateDetails(request.Name, request.Description);
            product.ChangePrice(new Money(request.Price, product.Price.Currency));
            await _uow.SaveChangesAsync(ct);

            return Result.Success(product.ToDto());
        }

        public async Task<Result> DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            var product = await _products.GetByIdAsync(id, ct);
            if (product is null)
                return Error.NotFound("Product not found.");

            product.Deactivate();
            await _uow.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}

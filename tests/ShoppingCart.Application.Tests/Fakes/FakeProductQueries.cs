using ShoppingCart.Application.Common;
using ShoppingCart.Application.Products;

namespace ShoppingCart.Application.Tests.Fakes
{
    public sealed class FakeProductQueries : IProductQueries
    {
        private readonly FakeProductRepository _repository;

        public FakeProductQueries(FakeProductRepository repository) => _repository = repository;

        // Recorded so tests can check what the service passed down.
        public int LastPage { get; private set; }
        public int LastPageSize { get; private set; }
        public string? LastSearch { get; private set; }

        public Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_repository.Store.FirstOrDefault(p => p.Id == id && p.IsActive)?.ToDto());

        public Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
        {
            (LastPage, LastPageSize, LastSearch) = (page, pageSize, search);

            var items = _repository.Store
                .Where(p => search is null || p.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Name)
                .Select(p => p.ToDto())
                .ToList();

            var pageItems = items.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(new PagedResult<ProductDto>(pageItems, page, pageSize, items.Count));
        }
    }
}

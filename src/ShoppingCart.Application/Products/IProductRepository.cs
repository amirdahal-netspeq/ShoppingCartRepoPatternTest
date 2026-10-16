using ShoppingCart.Application.Abstractions;
using ShoppingCart.Domain.Products;

namespace ShoppingCart.Application.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        /// <summary>Case-insensitive. Pass the product's own id as excludeId when updating.</summary>
        Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken ct = default);
    }
}

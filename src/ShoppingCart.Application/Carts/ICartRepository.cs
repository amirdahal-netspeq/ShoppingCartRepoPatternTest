using ShoppingCart.Application.Abstractions;
using ShoppingCart.Domain.Carts;

namespace ShoppingCart.Application.Carts
{
    public interface ICartRepository : IRepository<Cart>
    {
        /// <summary>Loads the customer's cart together with its items, or null if they have none.</summary>
        Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    }
}

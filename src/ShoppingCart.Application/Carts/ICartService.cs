using ShoppingCart.Application.Common;

namespace ShoppingCart.Application.Carts
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(Guid customerId, CancellationToken ct = default);
        Task<Result<CartDto>> AddItemAsync(Guid customerId, AddCartItemRequest request, CancellationToken ct = default);
        Task<Result<CartDto>> ChangeItemQuantityAsync(Guid customerId, Guid productId, int quantity, CancellationToken ct = default);
        Task<Result<CartDto>> RemoveItemAsync(Guid customerId, Guid productId, CancellationToken ct = default);
        Task<Result> ClearAsync(Guid customerId, CancellationToken ct = default);
    }
}

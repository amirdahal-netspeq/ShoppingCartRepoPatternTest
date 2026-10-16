using ShoppingCart.Application.Abstractions;
using ShoppingCart.Application.Common;
using ShoppingCart.Application.Products;
using ShoppingCart.Domain.Carts;

namespace ShoppingCart.Application.Carts
{
    public class CartService : ICartService
    {
        public const string DefaultCurrency = "INR";

        private readonly ICartRepository _carts;
        private readonly IProductRepository _products;
        private readonly IUnitOfWork _uow;

        public CartService(ICartRepository carts, IProductRepository products, IUnitOfWork uow)
            => (_carts, _products, _uow) = (carts, products, uow);

        public async Task<CartDto> GetCartAsync(Guid customerId, CancellationToken ct = default)
        {
            var cart = await _carts.GetByCustomerIdAsync(customerId, ct);
            return cart?.ToDto() ?? CartMappings.EmptyCart(customerId, DefaultCurrency);
        }

        public async Task<Result<CartDto>> AddItemAsync(Guid customerId, AddCartItemRequest request, CancellationToken ct = default)
        {
            // Rules that need data from more than one aggregate live here, not in Cart.
            var product = await _products.GetByIdAsync(request.ProductId, ct);
            if (product is null || !product.IsActive)
                return Error.NotFound("Product not found");

            var cart = await _carts.GetByCustomerIdAsync(customerId, ct);

            var alreadyInCart = cart?.Items.FirstOrDefault(i => i.ProductId == request.ProductId)?.Quantity ?? 0;
            var requested = alreadyInCart + request.Quantity;

            if (requested > Cart.MaxQuantityPerItem)
                return Error.Validation($"You can buy at most {Cart.MaxQuantityPerItem} of the same product.");
            if (!product.CanBePurchased(requested))
                return Error.Conflict($"Only {product.StockQuantity} of '{product.Name}' available.");
            if (product.Price.Currency != (cart?.Currency ?? DefaultCurrency))
                return Error.Validation("This product is priced in a different currency than your cart.");

            if (cart is null)
            {
                cart = new Cart(customerId, DefaultCurrency);
                await _carts.AddAsync(cart, ct);
            }

            cart.AddItem(product.Id, product.Name, product.Price, request.Quantity);
            await _uow.SaveChangesAsync(ct);

            return Result.Success(cart.ToDto());
        }

        public async Task<Result<CartDto>> ChangeItemQuantityAsync(Guid customerId, Guid productId, int quantity, CancellationToken ct = default)
        {
            if (quantity < 1 || quantity > Cart.MaxQuantityPerItem)
                return Error.Validation($"Quantity must be between 1 and {Cart.MaxQuantityPerItem}.");

            var cart = await _carts.GetByCustomerIdAsync(customerId, ct);
            if (cart is null || !cart.Contains(productId))
                return Error.NotFound("The product is not in your cart.");

            var product = await _products.GetByIdAsync(productId, ct);
            if (product is null || !product.CanBePurchased(quantity))
                return Error.Conflict("The requested quantity is not available.");

            cart.ChangeItemQuantity(productId, quantity);
            await _uow.SaveChangesAsync(ct);

            return Result.Success(cart.ToDto());
        }

        public async Task<Result<CartDto>> RemoveItemAsync(Guid customerId, Guid productId, CancellationToken ct = default)
        {
            var cart = await _carts.GetByCustomerIdAsync(customerId, ct);
            if (cart is null || !cart.Contains(productId))
                return Error.NotFound("The product is not in the cart.");

            cart.RemoveItem(productId);
            await _uow.SaveChangesAsync(ct);

            return Result.Success(cart.ToDto());
        }

        public async Task<Result> ClearAsync(Guid customerId, CancellationToken ct = default)
        {
            var cart = await _carts.GetByCustomerIdAsync(customerId, ct);
            if (cart is null)
                return Result.Success();

            cart.Clear();
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}

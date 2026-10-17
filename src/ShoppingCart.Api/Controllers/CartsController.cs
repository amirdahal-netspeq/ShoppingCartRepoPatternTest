using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Api.Common;
using ShoppingCart.Application.Carts;

namespace ShoppingCart.Api.Controllers
{
    // No authentication yet: customerId in the URL is a placeholder.
    // Once there is authentication, the customer comes from the token, never from the URL.
    [ApiController]
    [Route("api/customers/{customerId:guid}/cart")]
    public sealed class CartsController : ControllerBase
    {
        private readonly ICartService _carts;

        public CartsController(ICartService carts) => _carts = carts;

        [HttpGet]
        [ProducesResponseType<CartDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCart(Guid customerId, CancellationToken ct)
            => Ok(await _carts.GetCartAsync(customerId, ct));

        [HttpPost("items")]
        [ProducesResponseType<CartDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddItem(Guid customerId, AddCartItemRequest request, CancellationToken ct)
            => (await _carts.AddItemAsync(customerId, request, ct)).ToActionResult(this);

        [HttpPut("items/{productId:guid}")]
        [ProducesResponseType<CartDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ChangeItemQuantity(
            Guid customerId, Guid productId, ChangeCartItemQuantityRequest request, CancellationToken ct)
            => (await _carts.ChangeItemQuantityAsync(customerId, productId, request.Quantity, ct)).ToActionResult(this);

        [HttpDelete("items/{productId:guid}")]
        [ProducesResponseType<CartDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItem(Guid customerId, Guid productId, CancellationToken ct)
            => (await _carts.RemoveItemAsync(customerId, productId, ct)).ToActionResult(this);

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Clear(Guid customerId, CancellationToken ct)
            => (await _carts.ClearAsync(customerId, ct)).ToActionResult(this, "Cart cleared.");
    }
}

using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Api.Common;
using ShoppingCart.Application.Common;
using ShoppingCart.Application.Products;

namespace ShoppingCart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _products;

        public ProductsController(IProductService products) => _products = products;

        [HttpGet]
        [ProducesResponseType<PagedResult<ProductDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, CancellationToken ct = default)
            => this.OkResponse(await _products.GetPagedAsync(page, pageSize, search, ct));

        [HttpGet("{id:guid}")]
        [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
            => (await _products.GetByIdAsync(id, ct)).ToActionResult(this);

        [HttpPost]
        [ProducesResponseType<ProductDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct = default)
        {
            var result = await _products.CreateAsync(request, ct);
            return result.IsSuccess
                ? CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Value.Id },
                    ApiResponseDto<ProductDto>.Ok(result.Value, "Product created."))
                : result.Error!.ToFailure(this);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(Guid id, UpdateProductRequest request, CancellationToken ct)
        => (await _products.UpdateAsync(id, request, ct)).ToActionResult(this);
    }
}

using ShoppingCart.Application.Carts;
using ShoppingCart.Application.Common;
using ShoppingCart.Application.Tests.Fakes;
using ShoppingCart.Domain.Carts;
using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Products;

namespace ShoppingCart.Application.Tests
{
    public class CartServiceTests
    {
        private readonly FakeProductRepository _products = new();
        private readonly FakeCartRepository _carts = new();
        private readonly FakeUnitOfWork _uow = new();
        private readonly CartService _service;

        private readonly Guid _customer = Guid.NewGuid();

        public CartServiceTests()
            => _service = new CartService(_carts, _products, _uow);

        private Product Seed(decimal price = 10, int stock = 5, bool active = true)
        {
            var product = new Product("Keyboard", null, new Money(price, "INR"), stock);
            if (!active) product.Deactivate();
            _products.Store.Add(product);
            return product;
        }

        private Task<Result<CartDto>> Add(Product product, int quantity = 1)
            => _service.AddItemAsync(_customer, new AddCartItemRequest
            {
                ProductId = product.Id,
                Quantity = quantity
            });

        [Fact]
        public async Task AddItems_creates_a_cart_and_adds_the_line()
        {
            var product = Seed(price: 10m);

            var result = await Add(product, 2);

            Assert.True(result.IsSuccess);
            var line = Assert.Single(result.Value.Items);
            Assert.Equal(2, line.Quantity);
            Assert.Equal(20m, result.Value.Total);
            Assert.Single(_carts.Store);
            Assert.Equal(1, _uow.SaveCalls);
        }

        [Fact]
        public async Task AddItem_for_an_unknown_product_returns_not_found_and_creates_nothing()
        {
            var result = await _service.AddItemAsync(_customer, new AddCartItemRequest
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1
            });

            Assert.Equal(ErrorType.NotFound, result.Error!.Type);
            Assert.Empty(_carts.Store);
            Assert.Equal(0, _uow.SaveCalls);
        }

        [Fact]
        public async Task AddItem_for_an_inactive_product_returns_not_found()
        {
            var product = Seed(active: false);

            var result = await Add(product, 1);

            Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        }

        [Fact]
        public async Task AddItem_beyond_available_stock_returns_conflict_and_does_not_save()
        {
            var product = Seed(stock: 2);

            var result = await Add(product, 3);

            Assert.Equal(ErrorType.Conflict, result.Error!.Type);
            Assert.Empty(_carts.Store);
            Assert.Equal(0, _uow.SaveCalls);
        }

        [Fact]
        public async Task AddItem_counts_what_is_already_in_the_cart_against_stock()
        {
            var product = Seed(stock: 3);
            await Add(product, 2);

            var second = await Add(product, 2);    // 2 already in the cart + 2 = 4 > 3

            Assert.Equal(ErrorType.Conflict, second.Error!.Type);
            var line = Assert.Single(Assert.Single(_carts.Store).Items);
            Assert.Equal(2, line.Quantity);
        }

        [Fact]
        public async Task AddItem_beyond_the_per_product_maximum_returns_validation()
        {
            var product = Seed(stock: 100);
            await Add(product, Cart.MaxQuantityPerItem);

            var result = await Add(product, 1);    // already at max

            Assert.Equal(ErrorType.Validation, result.Error!.Type);
        }

        [Fact]
        public async Task ChangeItemQuantity_for_a_product_not_in_the_cart_returns_not_found()
        {
            var product = Seed();

            var result = await _service.ChangeItemQuantityAsync(_customer, product.Id, 1);

            Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        }

        [Fact]
        public async Task ChangeItemQuantity_updates_the_line_and_saves()
        {
            var product = Seed(price: 10m, stock: 10);
            await Add(product, 1);

            var result = await _service.ChangeItemQuantityAsync(_customer, product.Id, 4);

            Assert.True(result.IsSuccess);
            Assert.Equal(40, result.Value.Total);
            Assert.Equal(2, _uow.SaveCalls);
        }

        [Fact]
        public async Task RemoveItem_removes_the_line_and_saves()
        {
            var product = Seed();
            await Add(product, 1);

            var result = await _service.RemoveItemAsync(_customer, product.Id);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value.Items);
            Assert.Equal(2, _uow.SaveCalls);
        }

        [Fact]
        public async Task GetCart_for_a_customer_without_a_cart_returns_an_empty_cart_without_creating_one()
        {
            var cart = await _service.GetCartAsync(_customer);

            Assert.Empty(cart.Items);
            Assert.Equal(0m, cart.Total);
            Assert.Empty(_carts.Store);
            Assert.Equal(0, _uow.SaveCalls);
        }

        [Fact]
        public async Task Clear_for_a_customer_without_a_cart_succeeds_without_saving()
        {
            var result = await _service.ClearAsync(_customer);

            Assert.True(result.IsSuccess);
            Assert.Empty(_carts.Store);
            Assert.Equal(0, _uow.SaveCalls);
        }

        [Fact]
        public async Task Second_AddItem_reuses_the_existing_cart_rather_than_creating_a_new_one()
        {
            var product = Seed(price: 10m);
            await Add(product, 1);

            var result = await Add(product, 2);
            Assert.True(result.IsSuccess);
            Assert.Equal(30m, result.Value.Total);
            Assert.Single(_carts.Store);
            Assert.Equal(2, _uow.SaveCalls);
        }
    }
}

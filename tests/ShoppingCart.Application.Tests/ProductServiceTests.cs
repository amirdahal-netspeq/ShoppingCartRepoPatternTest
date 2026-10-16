using ShoppingCart.Application.Common;
using ShoppingCart.Application.Products;
using ShoppingCart.Application.Tests.Fakes;
using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Products;

namespace ShoppingCart.Application.Tests
{
    public class ProductServiceTests
    {
        private readonly FakeProductRepository _repository = new();
        private readonly FakeUnitOfWork _uow = new();
        private readonly FakeProductQueries _queries;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _queries = new FakeProductQueries(_repository);
            _service = new ProductService(_repository, _queries, _uow);
        }

        private Product Seed(string name = "Keyboard", decimal price = 49.99m, int stock = 5)
        {
            var product = new Product(name, null, new Money(price, "INR"), stock);
            _repository.Store.Add(product);
            return product;
        }

        [Fact]
        public async Task Create_saves_a_new_product()
        {
            var result = await _service.CreateAsync(new CreateProductRequest
            {
                Name = "  Keyboard ",
                Price = 49.99m,
                Currency = "inr",
                StockQuantity = 5
            });

            Assert.True(result.IsSuccess);
            Assert.Equal("Keyboard", result.Value.Name);
            Assert.Single(_repository.Store);
            Assert.Equal(1, _uow.SaveCalls);
        }

        [Fact]
        public async Task Create_with_an_existing_name_returns_conflict_and_does_not_save()
        {
            Seed("Keyboard");

            var result = await _service.CreateAsync(new CreateProductRequest
            {
                Name = "keyboard",
                Price = 20,
                StockQuantity = 1
            });

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.Conflict, result.Error!.Type);
            Assert.Single(_repository.Store);
            Assert.Equal(0, _uow.SaveCalls);
        }

        [Fact]
        public async Task Update_changes_the_details_and_saves_once()
        {
            var product = Seed();

            var result = await _service.UpdateAsync(product.Id, new UpdateProductRequest
            {
                Name = "Mechanical Keyboard",
                Price = 59.99m,
            });

            Assert.True(result.IsSuccess);
            Assert.Equal("Mechanical Keyboard", result.Value.Name);
            Assert.Equal(59.99m, result.Value.Price);
            Assert.Equal(1, _uow.SaveCalls);
        }

        [Fact]
        public async Task Update_of_an_unknown_task_returns_not_found()
        {
            var result = await _service.UpdateAsync(Guid.NewGuid(), new UpdateProductRequest
            {
                Name = "X",
                Price = 1,
            });

            Assert.Equal(ErrorType.NotFound, result.Error!.Type);
            Assert.Equal(0, _uow.SaveCalls);
        }

        [Fact]
        public async Task Update_cannot_take_the_name_of_another_product()
        {
            Seed("Keyboard");
            var mouse = Seed("Mouse");

            var result = await _service.UpdateAsync(mouse.Id, new UpdateProductRequest
            {
                Name = "Keyboard",
                Price = 5,
            });

            Assert.Equal(ErrorType.Conflict, result.Error!.Type);
        }

        [Fact]
        public async Task GetById_of_an_unknown_product_returns_not_found()
        {
            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        }

        [Fact]
        public async Task GetPaged_normalizes_the_paging_arguments_before_querying()
        {
            await _service.GetPagedAsync(page: 1, pageSize: 1000, search: " key ");

            Assert.Equal(1, _queries.LastPage);
            Assert.Equal(ProductService.MaxPageSize, _queries.LastPageSize);
            Assert.Equal("key", _queries.LastSearch);
        }

        [Fact]
        public async Task Deactivate_sets_the_product_to_inactive_and_saves()
        {
            var product = Seed();
            var result = await _service.DeactivateAsync(product.Id);

            Assert.True(result.IsSuccess);
            Assert.False(product.IsActive);
            Assert.Equal(1, _uow.SaveCalls);
        }

        [Fact]
        public async Task GetById_returns_not_found_for_inactive_product()
        {
            var product = Seed();
            await _service.DeactivateAsync(product.Id);

            var result = await _service.GetByIdAsync(product.Id);

            Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        }
    }
}

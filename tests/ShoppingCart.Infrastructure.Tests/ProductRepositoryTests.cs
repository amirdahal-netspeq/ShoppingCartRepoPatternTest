using ShoppingCart.Application.Common;
using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Products;
using ShoppingCart.Infrastructure.Persistence.Repositories;

namespace ShoppingCart.Infrastructure.Tests
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly SqlServerTestDatabase _db = new();
        public void Dispose() => _db.Dispose();

        private static Product NewProduct(string name = "Keyboard", decimal price = 49.99m, int stock = 10)
            => new(name, null, new Money(price, "INR"), stock);

        [Fact]
        public async Task A_saved_product_can_be_read_back_with_its_price()
        {
            var product = NewProduct();
            await using (var write = _db.NewContext())
            {
                await new ProductRepository(write).AddAsync(product);
                await write.SaveChangesAsync();
            }

            await using var read = _db.NewContext();
            var loaded = await new ProductRepository(read).GetByIdAsync(product.Id);

            Assert.NotNull(loaded);
            Assert.Equal("Keyboard", loaded!.Name);
            Assert.Equal(new Money(49.99m, "INR"), loaded.Price);
            Assert.Equal(10, loaded.StockQuantity);
        }

        [Fact]
        public async Task NameExists_ignores_case_and_can_exclude_the_product_itself()
        {
            var product = NewProduct("Keyboard");
            await using (var write = _db.NewContext())
            {
                await new ProductRepository(write).AddAsync(product);
                await write.SaveChangesAsync();
            }

            await using var read = _db.NewContext();
            var repo = new ProductRepository(read);

            Assert.True(await repo.NameExistsAsync("KEYBOARD", excludeId: null));
            Assert.False(await repo.NameExistsAsync("keyboard", excludeId: product.Id));
            Assert.False(await repo.NameExistsAsync("Mouse", excludeId: null));
        }

        [Fact]
        public async Task Database_rejects_two_products_with_same_name()
        {
            await using var context = _db.NewContext();
            var repo = new ProductRepository(context);

            await repo.AddAsync(NewProduct("Keyboard"));
            await repo.AddAsync(NewProduct("Keyboard"));

            await Assert.ThrowsAsync<DataConflictException>(() => context.SaveChangesAsync());
        }
    }
}

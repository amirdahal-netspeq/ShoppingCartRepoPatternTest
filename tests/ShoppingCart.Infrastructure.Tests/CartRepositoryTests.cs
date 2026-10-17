using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Common;
using ShoppingCart.Domain.Carts;
using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Products;
using ShoppingCart.Infrastructure.Persistence.Repositories;

namespace ShoppingCart.Infrastructure.Tests
{
    public class CartRepositoryTests : IDisposable
    {
        private readonly SqlServerTestDatabase _db = new();
        public void Dispose() => _db.Dispose();

        private static Money Inr(decimal amount) => new(amount, "INR");


        private async Task<Product> SeedProduct(string name = "Keyboard", decimal price = 49.99m)
        {
            var product = new Product(name, null, Inr(price), 100);
            await using var context = _db.NewContext();
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        private async Task SaveNewCart(Guid customer, params (Guid ProductId, string Name, decimal Price, int Qty)[] lines)
        {
            await using var write = _db.NewContext();
            var cart = new Cart(customer, "INR");
            foreach (var line in lines)
                cart.AddItem(line.ProductId, line.Name, Inr(line.Price), line.Qty);

            await new CartRepository(write).AddAsync(cart);
            await write.SaveChangesAsync();
        }

        [Fact]
        public async Task A_cart_is_saved_and_loaded_together_with_its_items()
        {
            var keyboard = await SeedProduct("Keyboard", 49.99m);
            var mouse = await SeedProduct("Mouse", 19.50m);

            var customer = Guid.NewGuid();
            await SaveNewCart(customer,
                (keyboard.Id, "Keyboard", 49.99m, 2),
                (mouse.Id, "Mouse", 19.50m, 1)
            );

            await using var read = _db.NewContext();
            var loaded = await new CartRepository(read).GetByCustomerIdAsync(customer);

            Assert.NotNull(loaded);
            Assert.Equal(2, loaded!.Items.Count);
            Assert.Equal(Inr(119.48m), loaded.Total);   // 2 x 49.99 + 19.50
        }

        [Fact]
        public async Task A_line_added_to_an_existing_cart_is_persisted()
        {
            var product = await SeedProduct();
            var customer = Guid.NewGuid();
            await SaveNewCart(customer);

            await using (var second = _db.NewContext())
            {
                var cart = await new CartRepository(second).GetByCustomerIdAsync(customer);
                cart!.AddItem(product.Id, "Keyboard", Inr(10), 1);
                await second.SaveChangesAsync();
            }

            await using var third = _db.NewContext();
            var loaded = await new CartRepository(third).GetByCustomerIdAsync(customer);
            Assert.Single(loaded!.Items);
        }

        [Fact]
        public async Task A_removed_line_is_deleted_from_the_database()
        {
            var keyboard = await SeedProduct("Keyboard", 49.99m);
            var mouse = await SeedProduct("Mouse", 19.50m);

            var customer = Guid.NewGuid();

            await SaveNewCart(customer,
                (keyboard.Id, "Keyboard", 49.99m, 2),
                (mouse.Id, "Mouse", 19.50m, 1)
            );

            await using (var change = _db.NewContext())
            {
                var cart = await new CartRepository(change).GetByCustomerIdAsync(customer);
                cart!.RemoveItem(keyboard.Id);
                await change.SaveChangesAsync();
            }

            await using var read = _db.NewContext();
            Assert.Equal(1, await read.Set<CartItem>().CountAsync());
        }

        [Fact]
        public async Task The_database_allows_only_one_cart_per_customer()
        {
            var customer = Guid.NewGuid();
            await using var context = _db.NewContext();
            var repository = new CartRepository(context);
            await repository.AddAsync(new Cart(customer, "USD"));
            await repository.AddAsync(new Cart(customer, "USD"));

            await Assert.ThrowsAsync<DataConflictException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task A_line_cannot_reference_a_product_that_does_not_exist()
        {
            await using var context = _db.NewContext();
            var cart = new Cart(Guid.NewGuid(), "INR");
            cart.AddItem(Guid.NewGuid(), "Ghost", Inr(10), 1);          // no such product
            await new CartRepository(context).AddAsync(cart);

            await Assert.ThrowsAsync<DataConflictException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task A_product_that_is_in_a_cart_cannot_be_deleted()
        {
            var product = await SeedProduct();
            await SaveNewCart(Guid.NewGuid(), (product.Id, "Keyboard", 10m, 1));

            await using var context = _db.NewContext();
            var loaded = await context.Products.FirstAsync(p => p.Id == product.Id);
            context.Products.Remove(loaded);

            await Assert.ThrowsAsync<DataConflictException>(() => context.SaveChangesAsync());
        }
    }
}

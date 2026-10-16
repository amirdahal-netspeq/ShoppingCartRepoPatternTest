using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Products;

namespace ShoppingCart.Domain.Tests
{
    public class ProductTests
    {
        private static Product NewProduct(int stock = 5)
            => new("Keyboard", "Mechanical", new Money(49.99m, "USD"), stock);

        [Fact]
        public void New_product_is_active_with_the_given_stock()
        {
            var product = NewProduct(stock: 5);

            Assert.True(product.IsActive);
            Assert.Equal(5, product.StockQuantity);
            Assert.NotEqual(Guid.Empty, product.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_rejects_a_blank_name(string name)
            => Assert.Throws<DomainException>(() => new Product(name, null, new Money(10, "USD"), 1));

        [Fact]
        public void RemoveStock_reduces_the_stock()
        {
            var product = NewProduct(stock: 5);

            product.RemoveStock(2);

            Assert.Equal(3, product.StockQuantity);
        }

        [Fact]
        public void RemoveStock_beyond_available_throws_and_leaves_stock_unchanged()
        {
            var product = NewProduct(stock: 3);

            Assert.Throws<DomainException>(() => product.RemoveStock(4));
            Assert.Equal(3, product.StockQuantity);
        }

        [Fact]
        public void An_inactive_product_cannot_be_purchased()
        {
            var product = NewProduct();
            product.Deactivate();

            Assert.False(product.CanBePurchased(1));
        }
    }
}

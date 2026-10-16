using ShoppingCart.Domain.Carts;
using ShoppingCart.Domain.Common;

namespace ShoppingCart.Domain.Tests
{
    public class CartTests
    {
        private static Money Usd(decimal amount) => new(amount, "USD");
        private static Cart NewCart() => new(Guid.NewGuid(), "USD");

        [Fact]
        public void AddItem_adds_a_new_line()
        {
            var cart = NewCart();
            var productId = Guid.NewGuid();

            cart.AddItem(productId, "Keyboard", Usd(49.99m), 2);

            var line = Assert.Single(cart.Items);
            Assert.Equal(productId, line.ProductId);
            Assert.Equal(2, line.Quantity);
            Assert.Equal(Usd(99.98m), line.LineTotal);
        }

        [Fact]
        public void AddItem_twice_for_the_same_product_merges_into_one_line()
        {
            var cart = NewCart();
            var productId = Guid.NewGuid();

            cart.AddItem(productId, "Keyboard", Usd(10), 2);
            cart.AddItem(productId, "Keyboard", Usd(10), 3);

            var line = Assert.Single(cart.Items);
            Assert.Equal(5, line.Quantity);
        }

        [Fact]
        public void AddItem_beyond_the_maximum_quantity_throws()
        {
            var cart = NewCart();
            var productId = Guid.NewGuid();
            cart.AddItem(productId, "Keyboard", Usd(10), Cart.MaxQuantityPerItem);

            Assert.Throws<DomainException>(() => cart.AddItem(productId, "Keyboard", Usd(10), 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddItem_rejects_a_non_positive_quantity(int quantity)
            => Assert.Throws<DomainException>(() => NewCart().AddItem(Guid.NewGuid(), "Keyboard", Usd(10), quantity));

        [Fact]
        public void AddItem_rejects_a_price_in_another_currency()
            => Assert.Throws<DomainException>(
                () => NewCart().AddItem(Guid.NewGuid(), "Keyboard", new Money(10, "EUR"), 1));

        [Fact]
        public void ChangeItemQuantity_updates_the_line_and_the_total()
        {
            var cart = NewCart();
            var productId = Guid.NewGuid();
            cart.AddItem(productId, "Keyboard", Usd(10), 1);

            cart.ChangeItemQuantity(productId, 4);

            Assert.Equal(Usd(40), cart.Total);
        }

        [Fact]
        public void RemoveItem_removes_the_line()
        {
            var cart = NewCart();
            var productId = Guid.NewGuid();
            cart.AddItem(productId, "Keyboard", Usd(10), 1);

            cart.RemoveItem(productId);

            Assert.True(cart.IsEmpty);
        }

        [Fact]
        public void RemoveItem_for_a_product_not_in_the_cart_throws()
            => Assert.Throws<DomainException>(() => NewCart().RemoveItem(Guid.NewGuid()));

        [Fact]
        public void Total_is_the_sum_of_all_line_totals()
        {
            var cart = NewCart();
            cart.AddItem(Guid.NewGuid(), "Keyboard", Usd(10.00m), 2);   // 20.00
            cart.AddItem(Guid.NewGuid(), "Mouse", Usd(5.50m), 1);       //  5.50

            Assert.Equal(Usd(25.50m), cart.Total);
        }

        [Fact]
        public void An_empty_cart_has_a_zero_total()
            => Assert.Equal(Usd(0), NewCart().Total);
    }
}

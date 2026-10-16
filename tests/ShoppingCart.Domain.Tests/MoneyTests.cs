using ShoppingCart.Domain.Common;

namespace ShoppingCart.Domain.Tests
{
    public class MoneyTests
    {
        [Fact]
        public void Constructor_rounds_to_two_decimals_and_normalizes_currency()
        {
            var money = new Money(10.005m, "usd");

            Assert.Equal(10.01m, money.Amount);
            Assert.Equal("USD", money.Currency);
        }

        [Fact]
        public void Constructor_rejects_negative_amount()
            => Assert.Throws<DomainException>(() => new Money(-1, "USD"));

        [Fact]
        public void Adding_different_currencies_throws()
            => Assert.Throws<DomainException>(() => new Money(5, "USD") + new Money(5, "EUR"));

        [Fact]
        public void Equality_is_by_value()
            => Assert.Equal(new Money(5, "USD"), new Money(5.00m, "usd"));
    }
}

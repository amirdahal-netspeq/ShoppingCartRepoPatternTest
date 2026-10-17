namespace ShoppingCart.Domain.Common
{
    public sealed record Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Money() // only for EF Core, which needs a way to create the object
        {
            Currency = default!;
        }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new DomainException("Money cannot be negative.");

            if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
                throw new DomainException("Currency must be a 3-letter code.");


            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
            Currency = currency.Trim().ToUpperInvariant();
        }

        public static Money Zero(string currency) => new(0, currency);

        public static Money operator +(Money left, Money right)
        {
            EnsureSameCurrency(left, right);
            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator *(Money money, int quantity)
        {
            if (quantity < 0)
                throw new DomainException("Quantity cannot be negative.");

            return new Money(money.Amount * quantity, money.Currency);
        }

        private static void EnsureSameCurrency(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new DomainException($"Cannot combine {a.Currency} and {b.Currency}");
        }

        public override string ToString() => $"{Amount:0.00} {Currency}";
    }
}

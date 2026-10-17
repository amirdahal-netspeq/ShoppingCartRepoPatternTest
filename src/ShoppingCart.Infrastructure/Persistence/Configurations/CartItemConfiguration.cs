using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Domain.Carts;
using ShoppingCart.Domain.Products;

namespace ShoppingCart.Infrastructure.Persistence.Configurations
{
    internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .ValueGeneratedNever();

            builder.Property(i => i.ProductId)
                .IsRequired();

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.ProductName)
                .IsRequired()
                .HasMaxLength(Product.MaxNameLength);

            builder.Property(i => i.Quantity)
                .IsRequired();

            builder.ComplexProperty(i => i.UnitPrice, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("UnitPrice")
                    .HasPrecision(18, 2);

                price.Property(p => p.Currency)
                    .HasColumnName("UnitCurrency")
                    .HasMaxLength(3);
            });

            builder.Ignore(i => i.LineTotal);   // computed, not stored
        }
    }
}

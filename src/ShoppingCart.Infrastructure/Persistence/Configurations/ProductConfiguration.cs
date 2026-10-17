using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Domain.Products;

namespace ShoppingCart.Infrastructure.Persistence.Configurations
{
    internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(Product.MaxNameLength);

            builder.Property(p => p.Description)
                .HasMaxLength(2000);

            builder.Property(p => p.StockQuantity)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.ComplexProperty(p => p.Price, price =>
            {
                price.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasPrecision(19, 2);

                price.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3);
            });

            builder.HasIndex(p => p.Name)
                .IsUnique();
        }
    }
}

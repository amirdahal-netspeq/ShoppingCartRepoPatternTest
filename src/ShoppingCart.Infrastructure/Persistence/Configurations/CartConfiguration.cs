using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Domain.Carts;

namespace ShoppingCart.Infrastructure.Persistence.Configurations
{
    internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            builder.Property(c => c.CustomerId)
                .IsRequired();

            builder.Property(c => c.Currency)
                .IsRequired()
                .HasMaxLength(3);

            builder.HasIndex(c => c.CustomerId)
                .IsUnique();

            builder.Ignore(c => c.Total);

            builder.HasMany(c => c.Items)
                .WithOne()
                .HasForeignKey("CartId")    // a shadow property: CartItem has no CartId member
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Items is exposed as read-only, so EF must read and write the private _items field instead.
            builder.Navigation(c => c.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}

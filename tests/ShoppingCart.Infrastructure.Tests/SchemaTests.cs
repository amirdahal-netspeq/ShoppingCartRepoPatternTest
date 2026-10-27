using Microsoft.EntityFrameworkCore;
using ShoppingCart.Domain.Carts;

namespace ShoppingCart.Infrastructure.Tests
{
    [Collection("Database")]
    public class SchemaTests
    {
        private readonly SqlServerFixture _db;
        public SchemaTests(SqlServerFixture db) => _db = db;

        [Fact]
        public async Task The_schema_copied_from_dev_has_every_table_and_column_the_model_maps()
        {
            await using var context = _db.NewContext();

            await context.Products.Take(1).ToListAsync();
            await context.Carts.Include(c => c.Items).Take(1).ToListAsync();
            await context.Set<CartItem>().Take(1).ToListAsync();
        }
    }
}

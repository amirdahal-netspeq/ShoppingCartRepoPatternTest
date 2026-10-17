using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Products;
using ShoppingCart.Infrastructure.Persistence.Queries;

namespace ShoppingCart.Infrastructure.Tests
{
    public class ProductQueriesTest : IDisposable
    {
        private readonly SqlServerTestDatabase _db = new();

        public void Dispose() => _db.Dispose();

        private async Task Seed(params string[] names)
        {
            await using var context = _db.NewContext();
            foreach (var name in names)
                context.Products.Add(new(name, null, new Money(10, "INR"), 1));

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Search_ignores_case_and_results_are_ordered_by_name()
        {
            await Seed("Gamma Keyboard", "Beta Mouse", "Alpha KEYBOARD", "Delta Monitor");
            await using var context = _db.NewContext();

            var page = await new ProductQueries(context).GetPagedAsync(page: 1, pageSize: 10, search: "keyboard");

            Assert.Equal(2, page.TotalCount);
            Assert.Equal(new[] { "Alpha KEYBOARD", "Gamma Keyboard" }, page.Items.Select(p => p.Name).ToList());
        }

        [Fact]
        public async Task Paging_returns_the_requested_slice_and_the_total()
        {
            await Seed("A", "B", "C", "D", "E");
            await using var context = _db.NewContext();

            var page = await new ProductQueries(context).GetPagedAsync(page: 2, pageSize: 2, search: null);

            Assert.Equal(new[] { "C", "D" }, page.Items.Select(p => p.Name).ToList());
            Assert.Equal(5, page.TotalCount);
            Assert.Equal(3, page.TotalPages);
        }

        [Fact]
        public async Task GetById_returns_a_dto_with_the_price_and_null_for_an_unknown_id()
        {
            var product = new Product("Keyboard", "Mechanical", new Money(49.99m, "INR"), 5);
            await using (var write = _db.NewContext())
            {
                write.Products.Add(product);
                await write.SaveChangesAsync();
            }

            await using var context = _db.NewContext();
            var queries = new ProductQueries(context);

            var dto = await queries.GetByIdAsync(product.Id);

            Assert.NotNull(dto);
            Assert.Equal(49.99m, dto!.Price);
            Assert.Equal("INR", dto!.Currency);

            Assert.Null(await queries.GetByIdAsync(Guid.NewGuid()));
        }
    }
}

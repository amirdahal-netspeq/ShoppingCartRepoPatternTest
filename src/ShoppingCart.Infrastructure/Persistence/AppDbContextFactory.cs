using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShoppingCart.Infrastructure.Persistence
{
    /// <summary>Used only by the EF Core tools (dotnet ef). The app itself gets its context from DI.</summary>
    public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=shoppingcart.db")
                .Options;

            return new AppDbContext(options);
        }
    }
}

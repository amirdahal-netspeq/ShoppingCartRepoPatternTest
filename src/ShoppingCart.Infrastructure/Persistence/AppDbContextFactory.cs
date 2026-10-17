using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShoppingCart.Infrastructure.Persistence
{
    /// <summary>Used only by the EF Core tools (dotnet ef). The app itself gets its context from DI.</summary>
    public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public const string ConnectionVariable = "SHOPPINGCART_CONNECTION";

        public AppDbContext CreateDbContext(string[] args)
        {
            var connectionString =
                Environment.GetEnvironmentVariable(ConnectionVariable)
                ?? "Server=(localdb)\\MSSQLLocalDB;Database=ShoppingCart_DesignTime;Trusted_Connection=True;TrustServerCertificate=True";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new AppDbContext(options);
        }
    }
}

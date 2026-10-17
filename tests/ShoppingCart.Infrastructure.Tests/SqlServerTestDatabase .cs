using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Infrastructure.Persistence;

namespace ShoppingCart.Infrastructure.Tests
{
    public sealed class SqlServerTestDatabase : IDisposable
    {
        // Override with an environment variable on machines without LocalDB (CI, Docker).
        private static readonly string Server =
            Environment.GetEnvironmentVariable("SHOPPINGCART_TEST_SQLSERVER")
            ?? "Server=(localdb)\\MSSQLLocalDB;Trusted_Connection=True;TrustServerCertificate=True";

        private readonly string _connectionString = new SqlConnectionStringBuilder(Server)
        {
            InitialCatalog = $"ShoppingCartTests_{Guid.NewGuid():N}"
        }.ConnectionString;

        public SqlServerTestDatabase()
        {
            using var context = NewContext();
            context.Database.EnsureCreated();                 // builds the schema from the model
        }

        // Each call is like a new web request: an empty change tracker over the same database.
        public AppDbContext NewContext()
            => new(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(_connectionString).Options);

        public void Dispose()
        {
            using var context = NewContext();
            context.Database.EnsureDeleted();
        }
    }
}

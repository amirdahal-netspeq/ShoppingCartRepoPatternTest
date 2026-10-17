using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Infrastructure.Persistence;

namespace ShoppingCart.Infrastructure.Tests
{
    public sealed class SqliteTestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection = new("DataSource=:memory:;Foreign Keys=True");

        public SqliteTestDatabase()
        {
            _connection.Open();                 // an in-memory database lives only while a connection is open
            using var context = NewContext();
            context.Database.EnsureCreated();   // builds the schema from the model
        }

        // Each call is like a new web request: an empty change tracker over the same database.
        public AppDbContext NewContext()
            => new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);

        public void Dispose() => _connection.Dispose();
    }
}

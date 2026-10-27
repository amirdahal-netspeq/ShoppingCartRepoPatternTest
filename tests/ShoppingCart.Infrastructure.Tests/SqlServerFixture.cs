using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Dac;
using Respawn;
using ShoppingCart.Infrastructure.Persistence;

namespace ShoppingCart.Infrastructure.Tests
{
    public sealed class SqlServerFixture : IAsyncLifetime
    {
        private const string DatabasePrefix = "ShoppingCartTests_";

        private readonly string _serverConnection =
            Environment.GetEnvironmentVariable("SHOPPINGCART_TEST_SQLSERVER")
            ?? "Server=(localdb)\\MSSQLLocalDB;Trusted_Connection=True;TrustServerCertificate=True";

        private readonly string _databaseName = $"{DatabasePrefix}{Guid.NewGuid():N}";
        private Respawner _respawner = default!;

        public string ConnectionString { get; private set; } = default!;

        public async Task InitializeAsync()
        {
            await DropAbandonedDatabasesAsync();

            var dacpac = Path.Combine(AppContext.BaseDirectory, "Database", "schema.dacpac");
            if (!File.Exists(dacpac))
                throw new FileNotFoundException("schema.dacpac was not found. Run tools/refresh-schema and commit the result.", dacpac);

            await Task.Run(() =>
            {
                var services = new DacServices(_serverConnection);
                using var package = DacPackage.Load(dacpac);
                services.Deploy(package, _databaseName, upgradeExisting: false);
            });

            ConnectionString = new SqlConnectionStringBuilder(_serverConnection)
            {
                InitialCatalog = _databaseName
            }.ConnectionString;


            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions { DbAdapter = DbAdapter.SqlServer });
        }

        public async Task ResetAsync()
        {
            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            await _respawner.ResetAsync(connection);
        }

        public AppDbContext NewContext()
            => new(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(ConnectionString).Options);

        public async Task DisposeAsync()
        {
            SqlConnection.ClearAllPools();
            await ExecuteOnServerAsync($"ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_databaseName}]");
        }

        private async Task DropAbandonedDatabasesAsync()
        {
            var names = new List<string>();
            await using (var connection = new SqlConnection(_serverConnection))
            {
                await connection.OpenAsync();
                await using var query = new SqlCommand(
                    "SELECT name FROM sys.databases WHERE name LIKE 'ShoppingCartTests[_]%' AND create_date < DATEADD(DAY, -1, GETDATE())", connection);

                await using var reader = await query.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    names.Add(reader.GetString(0));
            }

            foreach (var name in names)
                await ExecuteOnServerAsync($"ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{name}]");
        }

        private async Task ExecuteOnServerAsync(string sql)
        {
            await using var connection = new SqlConnection(_serverConnection);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }

    [CollectionDefinition("Database")]
    public sealed class DatabaseConnection : ICollectionFixture<SqlServerFixture> { }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Application.Abstractions;
using ShoppingCart.Application.Carts;
using ShoppingCart.Application.Products;
using ShoppingCart.Infrastructure.Persistence;
using ShoppingCart.Infrastructure.Persistence.Queries;
using ShoppingCart.Infrastructure.Persistence.Repositories;

namespace ShoppingCart.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));

            // Forward to the SAME context instance the repositories use. If the Unit of Work were a different
            // instance, repositories would add to one change tracker and SaveChanges would save another, empty one.
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductQueries, ProductQueries>();

            return services;
        }
    }
}

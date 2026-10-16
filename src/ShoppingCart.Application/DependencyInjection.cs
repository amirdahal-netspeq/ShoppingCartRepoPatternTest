using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Application.Carts;
using ShoppingCart.Application.Products;

namespace ShoppingCart.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            return services;
        }
    }
}

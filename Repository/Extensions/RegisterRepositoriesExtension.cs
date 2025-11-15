using Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Repository.Implementations;

namespace Repository.Extensions
{
    public static class RegisterRepositoriesExtension
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}

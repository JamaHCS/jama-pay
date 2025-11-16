using Domain.Interfaces.Providers;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Service.Implementations;
using Service.Providers;
using Service.Strategies;

namespace Service.Extensions
{
    public static class RegisterServicesExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IProviderSelectionStrategy, ProviderSelectionStrategy>();

            services.AddHttpClient<IPaymentProvider, PagaFacilProvider>();
            services.AddHttpClient<IPaymentProvider, CazaPagosProvider>();

            return services;
        }
    }
}

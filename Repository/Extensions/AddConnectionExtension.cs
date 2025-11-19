using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Context;

namespace Repository.Extensions
{
    public static class AddConnectionExtension
    {
        public static void AddConnection(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("Environment settings is incorrects");

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        }
    }
}

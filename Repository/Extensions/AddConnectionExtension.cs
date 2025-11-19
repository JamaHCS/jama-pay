using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository.Context;

namespace Repository.Extensions
{
    public static class AddConnectionExtension
    {
        public static void AddConnection(this IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                ?? throw new InvalidOperationException("Connection string not found. Set CONNECTION_STRING environment variable or DefaultConnection in appsettings.json.");

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        }
    }
}

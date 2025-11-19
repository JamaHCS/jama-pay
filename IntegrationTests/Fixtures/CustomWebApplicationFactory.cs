using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Context;

namespace IntegrationTests.Fixtures
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var dbContextService = services.SingleOrDefault(
                    d => d.ServiceType == typeof(AppDbContext));
                
                if (dbContextService != null)
                {
                    services.Remove(dbContextService);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Providers:PagaFacil:BaseUrl"] = "https://app-paga-chg-aviva.azurewebsites.net/",
                    ["Providers:CazaPagos:BaseUrl"] = "https://app-caza-chg-aviva.azurewebsites.net/"
                });
            });

            Environment.SetEnvironmentVariable("CONNECTION_STRING", "mock-for-tests");
            Environment.SetEnvironmentVariable("PAGAFACIL_API_KEY", "test-key");
            Environment.SetEnvironmentVariable("CAZAPAGOS_API_KEY", "test-key");
        }
    }
}

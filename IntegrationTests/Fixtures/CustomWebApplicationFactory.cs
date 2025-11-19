using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository.Context;
using System.Net;
using System.Text;
using System.Text.Json;

namespace IntegrationTests.Fixtures
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            Environment.SetEnvironmentVariable("CONNECTION_STRING", "mock-connection-string-for-tests");
            Environment.SetEnvironmentVariable("PAGAFACIL_API_KEY", "test-key");
            Environment.SetEnvironmentVariable("CAZAPAGOS_API_KEY", "test-key");

            builder.ConfigureServices(services =>
            {
                var descriptorsToRemove = new List<ServiceDescriptor>();
                
                foreach (var descriptor in services.ToList())
                {
                    var serviceType = descriptor.ServiceType;
                    var implementationType = descriptor.ImplementationType;
                    
                    if (serviceType == typeof(DbContextOptions<AppDbContext>) ||
                        serviceType == typeof(AppDbContext) ||
                        serviceType == typeof(DbContextOptions) ||
                        serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>) ||
                        serviceType.Namespace?.Contains("EntityFramework") == true ||
                        serviceType.Namespace?.Contains("SqlServer") == true ||
                        implementationType?.Namespace?.Contains("EntityFramework") == true ||
                        implementationType?.Namespace?.Contains("SqlServer") == true)
                    {
                        descriptorsToRemove.Add(descriptor);
                    }
                }

                foreach (var descriptor in descriptorsToRemove)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                }, ServiceLifetime.Scoped);

                var httpClientDescriptors = services.Where(d => 
                    d.ServiceType.IsGenericType &&
                    d.ServiceType.GetGenericTypeDefinition() == typeof(IHttpClientFactory) ||
                    (d.ServiceType.Name.Contains("PagaFacil") || d.ServiceType.Name.Contains("CazaPagos"))).ToList();
                
                foreach (var descriptor in httpClientDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddHttpClient<Service.Providers.PagaFacilProvider>(client =>
                {
                    client.BaseAddress = new Uri("https://app-paga-chg-aviva.azurewebsites.net/");
                }).ConfigurePrimaryHttpMessageHandler(() => new MockHttpMessageHandler());

                services.AddHttpClient<Service.Providers.CazaPagosProvider>(client =>
                {
                    client.BaseAddress = new Uri("https://app-caza-chg-aviva.azurewebsites.net/");
                }).ConfigurePrimaryHttpMessageHandler(() => new MockHttpMessageHandler());
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Providers:PagaFacil:BaseUrl"] = "https://app-paga-chg-aviva.azurewebsites.net/",
                    ["Providers:CazaPagos:BaseUrl"] = "https://app-caza-chg-aviva.azurewebsites.net/"
                });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);
            
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            
            return host;
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post && request.RequestUri?.AbsolutePath == "/Order")
            {
                var mockResponse = new
                {
                    id = "mock-12345",
                    status = "Created",
                    message = "Order created successfully"
                };

                var jsonContent = JsonSerializer.Serialize(mockResponse);
                var response = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                };

                return Task.FromResult(response);
            }

            if (request.Method == HttpMethod.Put && request.RequestUri?.AbsolutePath.Contains("/cancellation") == true)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }

            if (request.Method == HttpMethod.Put && request.RequestUri?.AbsolutePath.Contains("/pay") == true)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}

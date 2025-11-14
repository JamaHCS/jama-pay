using Microsoft.OpenApi;
using System.Reflection;

namespace API.extensions
{
    public static class SwaggerConfigurationExtension
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "jama-pay gateway API",
                    Version = "v1",
                    Description = "API for managing payment orders with multiple payment providers integration",
                    Contact = new OpenApiContact
                    {
                        Name = "Hector Escobedo",
                        Email = "hector@jamadev.com"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                options.IncludeXmlComments(xmlPath);
            });
        }

        public static void UseSwaggerConfiguration(this WebApplication app)
        {
            if(app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "jama-pay gateway API");
                    options.RoutePrefix = "swagger";
                    options.DocumentTitle = "jama-pay gateway API";

                    options.DefaultModelExpandDepth(2);
                    options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    options.DisplayRequestDuration();
                });
            }
        }
    }
}

using Domain.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Extensions
{
    public static class MappingExtension
    {
        public static void AddAutoMappers(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        }
    }
}

using Domain.DTO;
using Domain.DTO.Requests;
using Domain.Validations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Extensions
{
    public static class ValidationExtensions
    {
        public static void AddFluentValidations(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CreateOrderRequestDTO>, CreateOrderRequestValidator>();
            services.AddScoped<IValidator<ProductDTO>, ProductDTOValidator>();
        }
    }
}

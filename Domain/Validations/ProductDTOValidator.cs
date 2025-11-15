using Domain.DTO;
using FluentValidation;

namespace Domain.Validations
{
    public class ProductDTOValidator : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre del producto es requerido.")
                .MaximumLength(200)
                .WithMessage("El nombre debe de tener una longitud máxima de 200 caracteres.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage("El precio debe de ser mayor a 0.")
                .LessThanOrEqualTo(1000000)
                .WithMessage("El precio debe de ser menor a 1,000,000.");
        }
    }
}

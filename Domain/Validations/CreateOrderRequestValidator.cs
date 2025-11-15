using Domain.DTO.Requests;
using Domain.Enums;
using FluentValidation;

namespace Domain.Validations
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequestDTO>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.Method)
                .NotEqual(PaymentMethod.None)
                .WithMessage("El método de pago es requerido y no puede ser Ninguno.");

            RuleFor(x => x.Products)
                .NotNull()
                .WithMessage("La lista de productos es requerida")
                .NotEmpty()
                .WithMessage("Debes de insertar al menos un producto.");

            RuleForEach(x => x.Products)
                .SetValidator(new ProductDTOValidator());
        }
    }
}

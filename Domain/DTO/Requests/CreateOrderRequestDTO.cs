using Domain.Enums;

namespace Domain.DTO.Requests
{
    public class CreateOrderRequestDTO
    {
        public PaymentMethod Method { get; set; }
        public List<ProductDTO> Products { get; set; } = new();
    }
}

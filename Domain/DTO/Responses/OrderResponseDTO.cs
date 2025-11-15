using Domain.Enums;

namespace Domain.DTO.Responses
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public string? ProviderOrderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod Method { get; set; }
        public List<FeeDTO> Fees { get; set; } = new();
        public List<TaxDTO> Taxes { get; set; } = new();
        public List<ProductDTO> Products { get; set; } = new();
        public decimal TotalFees { get; set; }
        public decimal TotalTaxes { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

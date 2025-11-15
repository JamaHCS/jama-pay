namespace Domain.DTO
{
    public class ProviderOrderResponseDTO
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public List<ProviderFeeDTO> Fees { get; set; } = new();
        public List<ProviderTaxDTO>? Taxes { get; set; }
        public List<ProductDTO> Products { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}

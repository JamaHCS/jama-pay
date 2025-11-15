namespace Domain.DTO.Requests
{
    public class ProviderCreateOrderRequestDTO
    {
        public string Method { get; set; } = string.Empty;
        public List<ProductDTO> Products { get; set; } = new();
    }
}

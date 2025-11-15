using System.Text.Json.Serialization;

namespace Domain.DTO
{
    public class ProviderFeeDTO
    {
        public string? Name { get; set; }
        public string? Title { get; set; }
        public decimal Amount { get; set; }
        public string GetName() => !string.IsNullOrEmpty(Name) ? Name : Title ?? "Fee";
    }
}

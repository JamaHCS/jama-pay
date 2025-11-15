using Domain.DTO.Requests;
using Domain.DTO.Responses;
using Domain.Enums;

namespace Domain.Interfaces.Providers
{
    public interface IPaymentProvider
    {
        string ProviderName { get; }
        bool SupportsPaymentMode(PaymentMethod method);
        decimal CalculateFees(decimal amount, PaymentMethod method);
        Task<OrderResponseDTO> CreateOrderAsync(CreateOrderRequestDTO request);
        Task<OrderResponseDTO?> GetOrderAsync(string providerOrderId);
        Task<bool> CancelOrderAsync(string providerOrderId);
        Task<bool> PayOrderAsync(string providerOrderId);
    }
}

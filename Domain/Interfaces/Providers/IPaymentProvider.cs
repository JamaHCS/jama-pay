using Domain.DTO;
using Domain.DTO.Requests;
using Domain.DTO.Responses;
using Domain.Enums;

namespace Domain.Interfaces.Providers
{
    public interface IPaymentProvider
    {
        string ProviderName { get; }
        bool SupportsPaymentMethod(PaymentMethod method);
        decimal CalculateFees(decimal amount, PaymentMethod method);
        Task<ProviderOrderResponseDTO> CreateOrderAsync(CreateOrderRequestDTO request);
        Task<ProviderOrderResponseDTO?> GetOrderAsync(string providerOrderId);
        Task<bool> CancelOrderAsync(string providerOrderId);
        Task<bool> PayOrderAsync(string providerOrderId);
    }
}

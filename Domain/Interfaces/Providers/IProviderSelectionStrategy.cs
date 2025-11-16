using Domain.Enums;

namespace Domain.Interfaces.Providers
{
    public interface IProviderSelectionStrategy
    {
        Task<IPaymentProvider> SelectOptimalProviderAsync(decimal amount, PaymentMethod method, IEnumerable<IPaymentProvider> availableProviders);
    }
}

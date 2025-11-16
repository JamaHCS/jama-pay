using Domain.Enums;
using Domain.Interfaces.Providers;

namespace Service.Strategies
{
    public class ProviderSelectionStrategy : IProviderSelectionStrategy
    {
        public async Task<IPaymentProvider> SelectOptimalProviderAsync(decimal amount, PaymentMethod method, IEnumerable<IPaymentProvider> availableProviders)
        {
            var supportedProviders = availableProviders.Where(p => p.SupportsPaymentMethod(method)).ToList();

            if (!supportedProviders.Any()) throw new InvalidOperationException($"No payment provider supports payment method: {method}");

            if (supportedProviders.Count == 1) return supportedProviders.First();

            IPaymentProvider? optimalProvider = null;
            decimal lowestFee = decimal.MaxValue;

            foreach (var provider in supportedProviders)
            {
                var fee = provider.CalculateFees(amount, method);

                if (fee < lowestFee)
                {
                    lowestFee = fee;
                    optimalProvider = provider;
                }
            }

            if (optimalProvider == null) throw new InvalidOperationException("Failed to select an optimal provider");

            return optimalProvider;
        }
    }
}

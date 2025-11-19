using AutoMapper;
using Domain.DTO;
using Domain.DTO.Requests;
using Domain.Enums;
using Domain.Interfaces.Providers;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace Service.Providers
{
    public class CazaPagosProvider : IPaymentProvider
    {
        public string ProviderName => "CazaPagos";

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions;

        public CazaPagosProvider(HttpClient httpClient, IConfiguration configuration, IMapper mapper)
        {
            _httpClient = httpClient;
            _apiKey = Environment.GetEnvironmentVariable("CAZAPAGOS_API_KEY")
                      ?? throw new ArgumentNullException("CazaPagos API Key is not configured. Set CAZAPAGOS_API_KEY environment variable or Providers:CazaPagos:ApiKey in appsettings.json");

            _httpClient.BaseAddress = new Uri(configuration["Providers:CazaPagos:BaseUrl"]
                                              ?? "https://app-caza-chg-aviva.azurewebsites.net/");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public decimal CalculateFees(decimal amount, PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.Card => (amount >= 0 && amount < 1501) ? amount * 0.02m : (amount >= 1500 && amount < 5001) ? amount * 0.015m : amount * 0.005m,
                PaymentMethod.Transfer => (amount >= 0 && amount < 501) ? 5m : (amount >= 500 && amount < 1001) ? amount * 0.025m : amount * 0.02m,
                _ => throw new NotSupportedException($"Payment method {method} is not supported by {ProviderName}.")
            };
        }

        public async Task<bool> CancelOrderAsync(string providerOrderId)
        {
            try
            {
                var response = await _httpClient.PutAsync($"cancellation?id={providerOrderId}", null);
                response.EnsureSuccessStatusCode();
                
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Error calling {ProviderName} API: {ex.Message}", ex);
            }
        }

        public async Task<ProviderOrderResponseDTO> CreateOrderAsync(CreateOrderRequestDTO request)
        {
            try
            {
                var providerRequest = new ProviderCreateOrderRequestDTO
                {
                    Method = MapPaymentMethodToProviderString(request.Method),
                    Products = request.Products
                };

                var response = await _httpClient.PostAsJsonAsync("Order", providerRequest, _jsonOptions);
                response.EnsureSuccessStatusCode();

                var providerResponse = await response.Content.ReadFromJsonAsync<ProviderOrderResponseDTO>(_jsonOptions);

                if (providerResponse == null)
                    throw new InvalidOperationException($"Failed to create order in {ProviderName}");

                return providerResponse;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Error calling {ProviderName} API: {ex.Message}", ex);
            }
        }

        public Task<ProviderOrderResponseDTO?> GetOrderAsync(string providerOrderId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PayOrderAsync(string providerOrderId)
        {
            try
            {
                var response = await _httpClient.PutAsync($"payment?id={providerOrderId}", null);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Error calling {ProviderName} API: {ex.Message}", ex);
            }
        }

        public bool SupportsPaymentMethod(PaymentMethod method)
        {
            return method == PaymentMethod.Card || method == PaymentMethod.Transfer;
        }

        private static string MapPaymentMethodToProviderString(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.Card => "Card",
                PaymentMethod.Transfer => "Transfer",
                _ => throw new NotSupportedException($"Payment method {method} is not supported by CazaPagos")
            };
        }
    }
}

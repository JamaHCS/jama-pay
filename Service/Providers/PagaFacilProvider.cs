using AutoMapper;
using Domain.DTO;
using Domain.DTO.Requests;
using Domain.DTO.Responses;
using Domain.Enums;
using Domain.Interfaces.Providers;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace Service.Providers
{
    public class PagaFacilProvider : IPaymentProvider
    {
        public string ProviderName => "PagaFacil";

        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions;

        public PagaFacilProvider(HttpClient httpClient, IConfiguration configuration, IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
            _apiKey = configuration["Providers:PagaFacil:ApiKey"]
                      ?? throw new ArgumentNullException("PagaFacil API Key is not configured");

            _httpClient.BaseAddress = new Uri(configuration["PaymentProviders:PagaFacil:BaseUrl"]
                                              ?? "https://app-paga-chg-aviva.azurewebsites.net/");
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
                PaymentMethod.Card => amount * 0.01m,  
                PaymentMethod.Cash => 15.00m,           
                _ => throw new NotSupportedException($"Payment method {method} is not supported by {ProviderName}.")
            };
        }

        public Task<bool> CancelOrderAsync(string providerOrderId)
        {
            throw new NotImplementedException();
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

        public Task<bool> PayOrderAsync(string providerOrderId)
        {
            throw new NotImplementedException();
        }

        public bool SupportsPaymentMethod(PaymentMethod method)
        {
            return method == PaymentMethod.Card || method == PaymentMethod.Cash;
        }

        private static string MapPaymentMethodToProviderString(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.Card => "Card",      
                PaymentMethod.Cash => "Cash",
                _ => throw new NotSupportedException($"Payment method {method} is not supported by PagaFacil")
            };
        }
    }
}

using Domain.DTO;
using Domain.DTO.Requests;
using Domain.Enums;
using FluentAssertions;
using IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests
{
    public class OrderControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public OrderControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateOrder_WithValidRequest_ShouldReturn201Created()
        {
            var request = new CreateOrderRequestDTO
            {
                Method = PaymentMethod.Card,
                Products = new List<ProductDTO>
                {
                    new ProductDTO { Name = "Laptop Dell", UnitPrice = 1200 },
                    new ProductDTO { Name = "Mouse Logitech", UnitPrice = 50 }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/orders", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().MatchRegex("(PagaFacil|CazaPagos)");
            content.Should().Contain("\"success\":true");
        }

        [Fact]
        public async Task GetAllOrders_ShouldReturn200Ok()
        {
            var response = await _client.GetAsync("/api/orders");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("\"success\":true");
        }

        [Fact]
        public async Task GetOrderById_WithNonExistentOrder_ShouldReturn404NotFound()
        {
            var nonExistentOrderId = 99999;

            var response = await _client.GetAsync($"/api/orders/{nonExistentOrderId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("\"success\":false");
            content.Should().Contain("Order not found");
        }

        [Fact]
        public async Task CancelOrder_WithValidOrder_ShouldReturn200Ok()
        {
            var createRequest = new CreateOrderRequestDTO
            {
                Method = PaymentMethod.Card,
                Products = new List<ProductDTO>
                {
                    new ProductDTO { Name = "Laptop", UnitPrice = 1000 }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/orders", createRequest);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            
            var orderId = System.Text.Json.JsonDocument.Parse(createContent)
                .RootElement.GetProperty("value")
                .GetProperty("id")
                .GetInt32();

            var response = await _client.PutAsync($"/api/orders/cancel/{orderId}", null);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("\"success\":true");
        }

        [Fact]
        public async Task PayOrder_WithValidOrder_ShouldReturn200Ok()
        {
            var createRequest = new CreateOrderRequestDTO
            {
                Method = PaymentMethod.Card,
                Products = new List<ProductDTO>
                {
                    new ProductDTO { Name = "Mouse", UnitPrice = 50 }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/orders", createRequest);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            
            var orderId = System.Text.Json.JsonDocument.Parse(createContent)
                .RootElement.GetProperty("value")
                .GetProperty("id")
                .GetInt32();

            var response = await _client.PutAsync($"/api/orders/pay/{orderId}", null);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("\"success\":true");
        }
    }
}

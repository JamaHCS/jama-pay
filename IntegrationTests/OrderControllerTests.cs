using Domain.DTO;
using Domain.DTO.Requests;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests
{
    public class OrderControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public OrderControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateOrder_WithValidRequest_ShouldReturn201Created()
        {
            // Arrange
            var request = new CreateOrderRequestDTO
            {
                Method = PaymentMethod.Card,
                Products = new List<ProductDTO>
                {
                    new ProductDTO { Name = "Laptop Dell", UnitPrice = 1200 },
                    new ProductDTO { Name = "Mouse Logitech", UnitPrice = 50 }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().MatchRegex("(PagaFacil|CazaPagos)");
            content.Should().Contain("\"success\":true");
        }

        [Fact]
        public async Task GetAllOrders_ShouldReturn200Ok()
        {
            // Arrange - No requiere setup previo

            // Act
            var response = await _client.GetAsync("/api/orders");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("\"success\":true");
        }

        [Fact]
        public async Task GetOrderById_WithNonExistentOrder_ShouldReturn404NotFound()
        {
            // Arrange
            var nonExistentOrderId = 99999;

            // Act
            var response = await _client.GetAsync($"/api/orders/{nonExistentOrderId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("\"success\":false");
            content.Should().Contain("Order not found");
        }
    }
}

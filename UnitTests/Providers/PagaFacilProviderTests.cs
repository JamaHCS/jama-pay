using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Service.Providers;
using System.Net;

namespace UnitTests.Providers
{
    public class PagaFacilProviderTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly PagaFacilProvider _sut;

        public PagaFacilProviderTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://app-paga-chg-aviva.azurewebsites.net/")
            };

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["Providers:PagaFacil:BaseUrl"])
                .Returns("https://app-paga-chg-aviva.azurewebsites.net/");

            Environment.SetEnvironmentVariable("PAGAFACIL_API_KEY", "test-api-key");

            _sut = new PagaFacilProvider(_httpClient, _configurationMock.Object, null);
        }

        [Fact]
        public void CalculateFees_WithCard_ShouldReturn1Percent()
        {
            decimal amount = 1000m;

            var result = _sut.CalculateFees(amount, PaymentMethod.Card);

            result.Should().Be(10m);
        }

        [Fact]
        public async Task PayOrderAsync_WithValidId_ShouldReturnTrue()
        {
            var orderId = "12345";
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri.ToString().Contains($"pay?id={orderId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK));

            var result = await _sut.PayOrderAsync(orderId);

            result.Should().BeTrue();
        }
    }
}

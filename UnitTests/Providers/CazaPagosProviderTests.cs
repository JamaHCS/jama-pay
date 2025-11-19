using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Service.Providers;
using System.Net;

namespace UnitTests.Providers
{
    public class CazaPagosProviderTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly CazaPagosProvider _sut;

        public CazaPagosProviderTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://app-caza-chg-aviva.azurewebsites.net/")
            };

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["Providers:CazaPagos:BaseUrl"])
                .Returns("https://app-caza-chg-aviva.azurewebsites.net/");

            Environment.SetEnvironmentVariable("CAZAPAGOS_API_KEY", "test-api-key");

            _sut = new CazaPagosProvider(_httpClient, _configurationMock.Object, null);
        }

        [Theory]
        [InlineData(1000, 20)]     
        [InlineData(2000, 30)]     
        [InlineData(6000, 30)]     
        public void CalculateFees_WithCard_ShouldReturnCorrectFee(decimal amount, decimal expectedFee)
        {
            var result = _sut.CalculateFees(amount, PaymentMethod.Card);

            result.Should().Be(expectedFee);
        }

        [Fact]
        public async Task CancelOrderAsync_WithValidId_ShouldReturnTrue()
        {
            var orderId = "67890";
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri.ToString().Contains($"cancellation?id={orderId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var result = await _sut.CancelOrderAsync(orderId);

            result.Should().BeTrue();
        }
    }
}

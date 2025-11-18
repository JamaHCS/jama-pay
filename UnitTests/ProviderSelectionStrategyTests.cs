using Domain.Enums;
using Domain.Interfaces.Providers;
using FluentAssertions;
using Moq;
using Service.Strategies;

namespace UnitTests
{
    public class ProviderSelectionStrategyTests
    {
        private readonly ProviderSelectionStrategy _sut;
        private readonly Mock<IPaymentProvider> _pagaFacilMock;
        private readonly Mock<IPaymentProvider> _cazaPagosMock;

        public ProviderSelectionStrategyTests()
        {
            _sut = new ProviderSelectionStrategy();
            _pagaFacilMock = new Mock<IPaymentProvider>();
            _cazaPagosMock = new Mock<IPaymentProvider>();

            // Setup provider names
            _pagaFacilMock.Setup(p => p.ProviderName).Returns("PagaFacil");
            _cazaPagosMock.Setup(p => p.ProviderName).Returns("CazaPagos");
        }

        [Fact]
        public async Task SelectOptimalProvider_WithCard_ShouldSelectPagaFacilWhenCheaper()
        {
            // Arrange - PagaFacil 1% vs CazaPagos 2%
            decimal amount = 1000m;
            var paymentMethod = PaymentMethod.Card;

            _pagaFacilMock.Setup(p => p.SupportsPaymentMethod(PaymentMethod.Card)).Returns(true);
            _pagaFacilMock.Setup(p => p.CalculateFees(amount, PaymentMethod.Card)).Returns(10m); // 1%

            _cazaPagosMock.Setup(p => p.SupportsPaymentMethod(PaymentMethod.Card)).Returns(true);
            _cazaPagosMock.Setup(p => p.CalculateFees(amount, PaymentMethod.Card)).Returns(20m); // 2%

            var providers = new List<IPaymentProvider> { _pagaFacilMock.Object, _cazaPagosMock.Object };

            // Act
            var result = await _sut.SelectOptimalProviderAsync(amount, paymentMethod, providers);

            // Assert
            result.Should().NotBeNull();
            result.ProviderName.Should().Be("PagaFacil");
        }

        [Fact]
        public async Task SelectOptimalProvider_WithCash_ShouldSelectPagaFacilAsOnlySupported()
        {
            // Arrange - Solo PagaFacil soporta Cash
            decimal amount = 500m;
            var paymentMethod = PaymentMethod.Cash;

            _pagaFacilMock.Setup(p => p.SupportsPaymentMethod(PaymentMethod.Cash)).Returns(true);
            _pagaFacilMock.Setup(p => p.CalculateFees(amount, PaymentMethod.Cash)).Returns(15m);

            _cazaPagosMock.Setup(p => p.SupportsPaymentMethod(PaymentMethod.Cash)).Returns(false);

            var providers = new List<IPaymentProvider> { _pagaFacilMock.Object, _cazaPagosMock.Object };

            // Act
            var result = await _sut.SelectOptimalProviderAsync(amount, paymentMethod, providers);

            // Assert
            result.Should().NotBeNull();
            result.ProviderName.Should().Be("PagaFacil");
        }

        [Fact]
        public async Task SelectOptimalProvider_WithTransfer_ShouldSelectCazaPagosAsOnlySupported()
        {
            // Arrange - Solo CazaPagos soporta Transfer
            decimal amount = 2000m;
            var paymentMethod = PaymentMethod.Transfer;

            _pagaFacilMock.Setup(p => p.SupportsPaymentMethod(PaymentMethod.Transfer)).Returns(false);

            _cazaPagosMock.Setup(p => p.SupportsPaymentMethod(PaymentMethod.Transfer)).Returns(true);
            _cazaPagosMock.Setup(p => p.CalculateFees(amount, PaymentMethod.Transfer)).Returns(40m); // 2%

            var providers = new List<IPaymentProvider> { _pagaFacilMock.Object, _cazaPagosMock.Object };

            // Act
            var result = await _sut.SelectOptimalProviderAsync(amount, paymentMethod, providers);

            // Assert
            result.Should().NotBeNull();
            result.ProviderName.Should().Be("CazaPagos");
        }
    }
}

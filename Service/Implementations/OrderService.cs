using AutoMapper;
using Domain.DTO.Requests;
using Domain.DTO.Responses;
using Domain.Entities;
using Domain.Entities.Global;
using Domain.Interfaces.Providers;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Service.Implementations
{
    public class OrderService : IOrderService
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IProviderSelectionStrategy _providerStrategy;
        private readonly IEnumerable<IPaymentProvider> _paymentProviders;
        private readonly IMapper _mapper;

        public OrderService(
    IOrderRepository orderRepository,
    IProviderSelectionStrategy providerStrategy,
    IEnumerable<IPaymentProvider> paymentProviders,
    IMapper mapper)
        {
            _orderRepository = orderRepository;
            _providerStrategy = providerStrategy;
            _paymentProviders = paymentProviders;
            _mapper = mapper;
        }

        public Task<Result<bool>> CancelOrderAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<OrderResponseDTO>> CreateOrderAsync(CreateOrderRequestDTO request)
        {
            try
            {
                var amount = request.Products.Sum(p => p.UnitPrice);

                var optimalProvider = await _providerStrategy.SelectOptimalProviderAsync(
                    amount,
                    request.Method,
                    _paymentProviders);

                var providerResponse = await optimalProvider.CreateOrderAsync(request);

                var order = _mapper.Map<Order>(providerResponse);
                order.ProviderName = optimalProvider.ProviderName;
                order.Amount = amount;

                var savedOrder = await _orderRepository.CreateAsync(order);

                var response = _mapper.Map<OrderResponseDTO>(savedOrder);

                return Result<OrderResponseDTO>.Ok<OrderResponseDTO>(response, 201);
            }
            catch (Exception ex)
            {
                return Result<OrderResponseDTO>.Failure<OrderResponseDTO>($"Failed to create order: {ex.Message}");
            }
        }

        public Task<Result<List<OrderResponseDTO>>> GetAllOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<OrderResponseDTO>> GetOrderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> PayOrderAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

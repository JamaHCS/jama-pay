using AutoMapper;
using Domain.DTO.Requests;
using Domain.DTO.Responses;
using Domain.Entities;
using Domain.Entities.Global;
using Domain.Enums;
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

        public async Task<Result<bool>> CancelOrderAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);

                if (order is null) return Result<bool>.Failure<bool>("Order not found", false, 404);
                if(order.Status == OrderStatus.Paid) return Result<bool>.Failure<bool>("Cannot cancel a paid order", false, 400);
                if(order.Status == OrderStatus.Cancelled) return Result<bool>.Failure<bool>("Order already cancelled", false, 400);
                if (order.Status == OrderStatus.Paid) return Result<bool>.Failure<bool>("Cannot cancel a paid order", false, 400);
                
                var provider = _paymentProviders.FirstOrDefault(p => p.ProviderName == order.ProviderName);

                if (provider is null) return Result<bool>.Failure<bool>("Payment provider not found", false, 500);

                var cancelled = await provider.CancelOrderAsync(order.ProviderOrderId.ToString());
                
                if(!cancelled) return Result<bool>.Failure<bool>("Failed to cancel order with payment provider", false, 500);
                else
                {
                    order.Status = OrderStatus.Cancelled;
                    await _orderRepository.UpdateAsync(order);

                    return Result<bool>.Ok<bool>(true, 200);
                }
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure<bool>($"Failed to create order: {ex.Message}", false, 400);
            }
        }

        public async Task<Result<OrderDetailsResponseDTO>> CreateOrderAsync(CreateOrderRequestDTO request)
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

                var response = _mapper.Map<OrderDetailsResponseDTO>(savedOrder);

                return Result<OrderDetailsResponseDTO>.Ok<OrderDetailsResponseDTO>(response, 201);
            }
            catch (Exception ex)
            {
                return Result<OrderDetailsResponseDTO>.Failure<OrderDetailsResponseDTO>($"Failed to create order: {ex.Message}");
            }
        }

        public async Task<Result<List<OrderResponseDTO>>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                var response = _mapper.Map<List<OrderResponseDTO>>(orders);

                return Result<List<OrderResponseDTO>>.Ok<List<OrderResponseDTO>>(response);
            }
            catch (Exception ex)
            {
                return Result<List<OrderResponseDTO>>.Failure<List<OrderResponseDTO>>($"Failed to get orders: {ex.Message}");
            }
        }

        public async Task<Result<OrderDetailsResponseDTO>> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                
                if (order is null) return Result<OrderDetailsResponseDTO>.Failure<OrderDetailsResponseDTO>("Order not found", 404);
                else 
                {
                    var response = _mapper.Map<OrderDetailsResponseDTO>(order);

                    return Result<OrderDetailsResponseDTO>.Ok<OrderDetailsResponseDTO>(response);
                }

            }
            catch (Exception ex)
            {
                return Result<OrderDetailsResponseDTO>.Failure<OrderDetailsResponseDTO>($"Failed to get order: {ex.Message}");
            }
        }

        public Task<Result<bool>> PayOrderAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

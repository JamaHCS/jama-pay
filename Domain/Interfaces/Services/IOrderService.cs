using Domain.DTO.Requests;
using Domain.DTO.Responses;
using Domain.Entities.Global;

namespace Domain.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Result<OrderDetailsResponseDTO>> CreateOrderAsync(CreateOrderRequestDTO request);
        Task<Result<List<OrderResponseDTO>>> GetAllOrdersAsync();
        Task<Result<OrderDetailsResponseDTO>> GetOrderByIdAsync(int id);
        Task<Result<bool>> CancelOrderAsync(int id);
        Task<Result<bool>> PayOrderAsync(int id);
    }
}

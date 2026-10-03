using Core.DTO;
using Core.Entities.Order;

namespace Core.Services
{
    public interface IOrderService
    {
        Task<Orders> CreateOrdersAsync(OrderDTO orderDTO, string BuyerEmail);
        Task<IReadOnlyList<OrderToReturnDTO>> GetAllOrdersForUserAsync(string BuyerEmail);
        Task<OrderToReturnDTO> GetOrderByIdAsync(int Id, string BuyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync();
    }
}

using AppProject.Client.Models.Response.Orders;
using AppProject.Client.Models.ViewModels.Orders;

namespace AppProject.Client.Service.Orders;

public interface IOrderService
{
    Task<OrdersCreateRS> CreateOrdersAsync(OrderVM orderVm);
    Task<IEnumerable<OrderRS>> GetAllOrdersForUserAsync();
    Task<OrderRS> GetOrderByIdAsync(int id);
    Task<IEnumerable<DeliveryRS>> GetDeliveryMethodAsync();
}
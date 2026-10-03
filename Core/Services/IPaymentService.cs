using Core.Entities;
using Core.Entities.Order;

namespace Core.Services
{
    public interface IPaymentService
    {
        Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId, int? deliveryId);
        // Task<Orders> UpdateOrderSuccess(string PaymentInten);
        // Task<Orders> UpdateOrderFaild(string PaymentInten);
        
        Task<Orders?> UpdateOrderSuccess(int orderId, string paymentIntentId);
        Task<Orders?> UpdateOrderFaild(int orderId);
    }
}

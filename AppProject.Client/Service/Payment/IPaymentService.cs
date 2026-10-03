using AppProject.Client.Models.Response.Orders;
using AppProject.Client.Models.Response.Payment;

namespace AppProject.Client.Service.Payment;

public interface IPaymentService
{
    Task<CustomerBasketRS> CreateOrUpdatePaymentAsync(string basketId, int? deliveryId);
    Task<string?> CreateCheckoutSessionAsync(string basketId, int? deliveryId, int orderId);    
}
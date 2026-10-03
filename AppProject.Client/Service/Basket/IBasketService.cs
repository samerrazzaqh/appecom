using AppProject.Client.Models.Response.Basket;
using AppProject.Client.Models.ViewModels.Basket;

namespace AppProject.Client.Service.Basket;

public interface IBasketService
{
    Task<BasketRS> GetBasketAsync(string id);
    Task<BasketRS> AddOrUpdateBasketAsync(CustomerBasketVM basket);
    Task<bool> DeleteBasketAsync(string id);
    
}
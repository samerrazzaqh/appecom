using System.Net.Http.Json;
using AppProject.Client.Models.Response.Basket;
using AppProject.Client.Models.ViewModels.Basket;
using Blazored.LocalStorage;
using Newtonsoft.Json;

namespace AppProject.Client.Service.Basket;

public class BasketService : IBasketService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public BasketService(HttpClient httpClient,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }


    public async Task<BasketRS> GetBasketAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync( $"api/Baskets/get-basket-item/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var basketRs = JsonConvert.DeserializeObject<BasketRS>(content);
                if (basketRs == null || string.IsNullOrEmpty(basketRs.Id))
                {
                    return null;
                }

                return basketRs;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<BasketRS> AddOrUpdateBasketAsync(CustomerBasketVM basket)
    {
        try
        {
            var response =
                await _httpClient.PostAsJsonAsync( "api/Baskets/update-basket", basket);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<BasketRS>(content);
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<BasketRS>(content);
                return errorResponse ?? new BasketRS
                {
                    StatusCode = (int)response.StatusCode,
                    Message = response.ReasonPhrase
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return new BasketRS
            {
                StatusCode = 500,
                Message = ex.Message
            };
        }
    }

    public async Task<bool> DeleteBasketAsync(string id)
    {
        try
        {
            var response =
                await _httpClient.DeleteAsync( $"api/Baskets/delete-basket-item/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting basket: {e.Message}");
            return false;
        }
    }
}
using System.Net.Http.Json;
using AppProject.Client.Models.Response.Orders;
using AppProject.Client.Models.ViewModels.Orders;
using Blazored.LocalStorage;
using Newtonsoft.Json;

namespace AppProject.Client.Service.Orders;

public class OrderService : IOrderService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public OrderService(HttpClient httpClient,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }


    public async Task<OrdersCreateRS?> CreateOrdersAsync(OrderVM orderVm)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Orders/create-order", orderVm);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var ordersCreateRs = JsonConvert.DeserializeObject<OrdersCreateRS>(content);
                return ordersCreateRs;
            }

            Console.WriteLine($"API Error Status: {response.StatusCode}, Content: {content}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in CreateOrdersAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<OrderRS>> GetAllOrdersForUserAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync( "api/Orders/get-orders-for-user");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<OrderRS>>(content);
                return result ?? new List<OrderRS>();
            }

            Console.WriteLine($"API Error ({response.StatusCode}): {content}");
            return Enumerable.Empty<OrderRS>();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Deserialization/Network Exception: {e.Message}");
            return Enumerable.Empty<OrderRS>();
        }
    }

    public async Task<OrderRS?> GetOrderByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Orders/get-order-by-id/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var orderRs = JsonConvert.DeserializeObject<OrderRS>(content);
                return orderRs;
            }
            Console.WriteLine($"API Error Status: {response.StatusCode}, Content: {content}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in GetOrderByIdAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<DeliveryRS>> GetDeliveryMethodAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync( "api/Orders/get-delivery");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<DeliveryRS>>(content);
                return result ?? new List<DeliveryRS>();
            }

            Console.WriteLine($"API Error ({response.StatusCode}): {content}");
            return Enumerable.Empty<DeliveryRS>();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Deserialization/Network Exception: {e.Message}");
            return Enumerable.Empty<DeliveryRS>();
        }
    }
    
    
    
    
    
}
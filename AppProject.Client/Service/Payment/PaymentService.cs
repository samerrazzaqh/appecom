using System.Net.Http.Json;
using System.Text.Json;
using AppProject.Client.Models.Response.Payment;
using Blazored.LocalStorage;

namespace AppProject.Client.Service.Payment;

public class PaymentService :IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public PaymentService(HttpClient httpClient,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }
    
    
    
    public async Task<CustomerBasketRS?> CreateOrUpdatePaymentAsync(string basketId, int? deliveryId)
    {
        try
        {
            // 1. بناء الـ Query String ليطابق مظهر Postman والـ Controller
            var queryParams = new List<string>();
        
            if (!string.IsNullOrEmpty(basketId)) 
                queryParams.Add($"basketId={Uri.EscapeDataString(basketId)}");
            
            if (deliveryId.HasValue) 
                queryParams.Add($"deliveryId={deliveryId.Value}");

            var queryString = string.Join("&", queryParams);
            var url = $"api/Payments/Create?{queryString}";

            // 2. إرسال طلب POST
            var response = await _httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                // 3. قراءة النتيجة وتحويل الـ JSON إلى CustomerBasketRS
                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                };
            
                var result = await response.Content.ReadFromJsonAsync<CustomerBasketRS>(options);
                return result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error ({response.StatusCode}): {errorContent}");
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in CreateOrUpdatePaymentAsync: {e.Message}");
            return null;
        }
    }

    public async Task<string?> CreateCheckoutSessionAsync(string basketId, int? deliveryId, int orderId)
    {
        try
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(basketId))
                queryParams.Add($"basketId={Uri.EscapeDataString(basketId)}");

            if (deliveryId.HasValue)
                queryParams.Add($"deliveryId={deliveryId.Value}");

            // إضافة orderId إلى الـ Query Parameters
            queryParams.Add($"orderId={orderId}");

            var queryString = string.Join("&", queryParams);
            var url = $"api/Payments/create-checkout-session?{queryString}";

            var response = await _httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadFromJsonAsync<StripeSessionResponse>(options);
                return result?.Url;
            }
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in CreateCheckoutSessionAsync: {e.Message}");
            return null;
        }
    }    
}
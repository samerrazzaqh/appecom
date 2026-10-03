using System.Net.Http.Json;
using AppProject.Client.Models.Response.Rating;
using AppProject.Client.Models.ViewModels.Rating;
using Blazored.LocalStorage;
using Newtonsoft.Json;

namespace AppProject.Client.Service.Rating;

public class RatingService : IRatingService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public RatingService(HttpClient httpClient,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    
    
    public async Task<bool> AddRatingAsync(RatingVM ratingVm)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Ratings/add-rating", ratingVm);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (bool.TryParse(content, out bool result))
                {
                    return result;
                }
            
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AddRatingAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<RatingRS>> GetAllRatingForProductAsync(int productId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Ratings/get-rating/{productId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ratingRS = JsonConvert.DeserializeObject<IEnumerable<RatingRS>>(content);
            
                return ratingRS ?? Enumerable.Empty<RatingRS>();
            }
            return Enumerable.Empty<RatingRS>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in GetAllRatingForProductAsync: {ex.Message}");
            return Enumerable.Empty<RatingRS>();
        }
    }

   
}
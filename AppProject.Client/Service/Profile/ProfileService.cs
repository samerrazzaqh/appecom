using System.Net.Http.Json;
using AppProject.Client.Models.Response.Profile;
using AppProject.Client.Models.ViewModels.Profile;
using Blazored.LocalStorage;
using Newtonsoft.Json;

namespace AppProject.Client.Service.Profile;

public class ProfileService : IProfileService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public ProfileService(HttpClient httpClient,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }


    //==================================getUserInfoAddress=============================================
    public async Task<AddressRS?> getUserInfoAddress()
    {
        try
        {
            // ===============fro localStorage==============================
            // await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            
            var response =
                await _httpClient.GetAsync( "api/Account/get-address-for-user");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var addressRS = JsonConvert.DeserializeObject<AddressRS>(content);
                return addressRS;
            }
            else
            {
                Console.WriteLine($"response Exception: {response.ReasonPhrase}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }


    //==================================UpdateAddress=============================================
    public async Task<bool> UpdateAddress(ShipAddressVM addressVM)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            
            
            var response = await _httpClient.PutAsJsonAsync( $"api/Account/update-address",
                addressVM);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Exception: {response.ReasonPhrase}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }
    
    
    
    
}
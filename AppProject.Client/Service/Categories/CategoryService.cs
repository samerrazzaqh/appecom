using System.Net.Http.Json;
using AppProject.Client.Models.Response.Categories;
using AppProject.Client.Models.ViewModels.Categories;
using Blazored.LocalStorage;
using Newtonsoft.Json;

namespace AppProject.Client.Service.Categories;

public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public CategoryService(HttpClient httpClient,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }


    //==================================GetCategories=============================================
    public async Task<IEnumerable<CategoriesRS>?> GetCategories()
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);


            var response = await _httpClient.GetAsync("api/Categories/get-all");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var categoriesRS = JsonConvert.DeserializeObject<IEnumerable<CategoriesRS>>(content);
                return categoriesRS;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<IEnumerable<CategoriesRS>>(content);
                return errorResponse ?? new List<CategoriesRS>
                {
                    new CategoriesRS
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = response.RequestMessage.ToString()
                    }
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<CategoriesRS> CreateCategory(CategoryVM newCategory)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            
            var response = await _httpClient.PostAsJsonAsync( "api/Categories/add-category",newCategory);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var categoriesRs = JsonConvert.DeserializeObject<CategoriesRS>(content);
                return categoriesRs;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<CategoriesRS>(content);
                return errorResponse ?? new CategoriesRS
                {
                    StatusCode = (int)response.StatusCode,
                    Message = errorResponse.Message
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<CategoriesRS?> GetCategory(int id)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            
            var response = await _httpClient.GetAsync( $"api/Categories/get-by-id/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var categoriesRs = JsonConvert.DeserializeObject<CategoriesRS>(content);
                return categoriesRs;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<CategoriesRS>(content);
                return errorResponse ?? new CategoriesRS
                {
                    StatusCode = (int)response.StatusCode,
                    Message = errorResponse.Message
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<CategoriesRS?> UpdateCategory(CategoryVM updatedCategory)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            
            var response = await _httpClient.PutAsJsonAsync("api/Categories/update-category",updatedCategory);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var categoriesRs = JsonConvert.DeserializeObject<CategoriesRS>(content);
                return categoriesRs;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<CategoriesRS>(content);
                return errorResponse ?? new CategoriesRS
                {
                    StatusCode = (int)response.StatusCode,
                    Message = errorResponse.Message
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }

    public async Task DeleteCategory(int id)
    {
        try
        {
            // ===============fro localStorage==============================
           // await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            
            await _httpClient.DeleteAsync( $"api/Categories/delete-category/{id}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    
    
}
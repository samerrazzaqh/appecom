using AppProject.Client.Models.Response.Product;
using AppProject.Client.Models.ViewModels.Product;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Globalization;
using AppProject.Client.Authentication;
using AppProject.Client.Helpers;


namespace AppProject.Client.Service.Products;

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public ProductsService(HttpClient httpClient,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }


    //==================================GetProducts=============================================

    public async Task<ProductRS<DataProduct>?> GetProducts(ProductVM productVm)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);

            
            
            var queryParams = new Dictionary<string, string?>();

            if (!string.IsNullOrEmpty(productVm.Sort)) queryParams["Sort"] = productVm.Sort;
            if (productVm.CategoryId.HasValue) queryParams["CategoryId"] = productVm.CategoryId.Value.ToString();
            if (!string.IsNullOrEmpty(productVm.Search)) queryParams["Search"] = productVm.Search;
            if (productVm.pageSize > 0) queryParams["pageSize"] = productVm.pageSize.ToString();
            if (productVm.PageNumber > 0) queryParams["PageNumber"] = productVm.PageNumber.ToString();

            var baseUrl = $"api/Products/get-all";
            var urlWithQuery = QueryHelpers.AddQueryString(baseUrl, queryParams);

            var response = await _httpClient.GetAsync(urlWithQuery);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ProductRS<DataProduct>>(content);
                return result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error (400)==========================: {errorContent}");
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception=================" + e.Message);
            return null;
        }
    }
    //==================================AddAsync Products=============================================

    public async Task<bool> AddAsync(AddProductVM addProductVm)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            

            using var content = new MultipartFormDataContent();

            if (!string.IsNullOrEmpty(addProductVm.Name)) content.Add(new StringContent(addProductVm.Name), "Name");
            if (!string.IsNullOrEmpty(addProductVm.Description))
                content.Add(new StringContent(addProductVm.Description), "Description");
            if (addProductVm.NewPrice > 0) content.Add(new StringContent(addProductVm.NewPrice.ToString()), "NewPrice");
            if (addProductVm.OldPrice > 0) content.Add(new StringContent(addProductVm.OldPrice.ToString()), "OldPrice");
            if (addProductVm.CategoryId.HasValue)
                content.Add(new StringContent(addProductVm.CategoryId.Value.ToString()), "CategoryId");

            if (addProductVm.Photo != null && addProductVm.Photo.Any())
            {
                foreach (var file in addProductVm.Photo)
                {
                    var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 15);
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "Photo", file.Name);
                }
            }

            var url = "api/Products/Add-Product";
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error (400)==========================: {errorContent}");
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    //==================================GetByIdAsync Products=============================================

    public async Task<GetProductRS> GetByIdAsync(int Id)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            

            var response = await _httpClient.GetAsync( $"api/Products/get-by-id/{Id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var getProductRs = JsonConvert.DeserializeObject<GetProductRS>(content);
                return getProductRs;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
    //==================================UpdateAsync Products=============================================

    public async Task<bool> UpdateAsync(UpdateProductVM updateProductVM)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(updateProductVM.Id.ToString()), "Id");
            content.Add(new StringContent(updateProductVM.Name ?? string.Empty), "Name");
            content.Add(new StringContent(updateProductVM.Description ?? string.Empty), "Description");

            if (updateProductVM.NewPrice.HasValue)
            {
                content.Add(new StringContent(updateProductVM.NewPrice.Value.ToString(CultureInfo.InvariantCulture)),
                    "NewPrice");
            }

            if (updateProductVM.OldPrice.HasValue)
            {
                content.Add(new StringContent(updateProductVM.OldPrice.Value.ToString(CultureInfo.InvariantCulture)),
                    "OldPrice");
            }

            if (updateProductVM.CategoryId.HasValue)
            {
                content.Add(new StringContent(updateProductVM.CategoryId.Value.ToString()), "CategoryId");
            }

            if (updateProductVM.Photo != null && updateProductVM.Photo.Any())
            {
                foreach (var file in updateProductVM.Photo)
                {
                    var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 15);
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "Photo", file.Name);
                }
            }

            var url = "api/Products/Update-Product";
            var response = await _httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Error ---------------- ({response.StatusCode}): {errorContent}");
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in UpdateAsync:---------------- {e.Message}");
            return false;
        }
    }
    //==================================DeleteAsync Products=============================================

    public async Task DeleteAsync(int productId)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            

            await _httpClient.DeleteAsync( $"api/Products/Delete-Product/{productId}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    
    
    
}
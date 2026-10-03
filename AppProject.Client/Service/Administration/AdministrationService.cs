using System.Net.Http.Json;
using AppProject.Client.Authentication;
using AppProject.Client.Helpers;
using AppProject.Client.Models.Response.Administration;
using AppProject.Client.Models.ViewModels.Administration;
using Blazored.LocalStorage;
using Newtonsoft.Json;

namespace AppProject.Client.Service.Administration;

public class AdministrationService : IAdministrationService
{
    
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public AdministrationService(HttpClient httpClient,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }
    
    
    // ==============================Roles===============================================================

    public async Task<IEnumerable<RolesRS>> ListRoles()
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);


            var response = await _httpClient.GetAsync( "api/Administration/GetListRoles");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var rolesRS = JsonConvert.DeserializeObject<IEnumerable<RolesRS>>(content);
                return rolesRS;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<IEnumerable<RolesRS>>(content);
                return errorResponse ?? new List<RolesRS>
                {
                    new RolesRS
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = response.RequestMessage.ToString()
                    }
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<RolesResponse> CreateRole(RolesVM rolesVm)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            var response = await _httpClient.PostAsJsonAsync("api/Administration/CreateRole", rolesVm);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<RolesResponse>(content)!;
            }

            var errorResponse = JsonConvert.DeserializeObject<RolesResponse>(content);
            string apiMessage = !string.IsNullOrWhiteSpace(errorResponse?.Message) 
                ? errorResponse.Message 
                : "Failed to create role";

            return new RolesResponse
            {
                StatusCode = (int)response.StatusCode,
                Message = apiMessage
            };
        }
        catch (Exception ex)
        {
            return new RolesResponse { StatusCode = 500, Message = ex.Message };
        }
    }

    public async Task<RolesResponse> DeleteRole(string roleId)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            var response = await _httpClient.DeleteAsync($"api/Administration/DeleteRole/{roleId}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<RolesResponse>(content)!;
            }

            var errorResponse = JsonConvert.DeserializeObject<RolesResponse>(content);
            string apiMessage = !string.IsNullOrWhiteSpace(errorResponse?.Message) 
                ? errorResponse.Message 
                : "Failed to delete role";

            return new RolesResponse
            {
                StatusCode = (int)response.StatusCode,
                Message = apiMessage
            };
        }
        catch (Exception ex)
        {
            return new RolesResponse { StatusCode = 500, Message = ex.Message };
        }
    }

    
    public async Task<RolesResponse<DataRoles>> GetUsersInRole(RolesVM rolesVm)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            var response = await _httpClient.PostAsJsonAsync( "api/Administration/GetUsersInRole", rolesVm);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<RolesResponse<DataRoles>>(content)!;
            }

            return new RolesResponse<DataRoles>
            {
                StatusCode = (int)response.StatusCode,
                Message = "Failed to load data"
            };
        }
        catch (Exception ex)
        {
            return new RolesResponse<DataRoles>
            {
                StatusCode = 500,
                Message = ex.Message
            };
        }
    }
    

    public async Task<RolesResponse> EditRole(RolesEditVM rolesEditDto)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            var response = await _httpClient.PutAsJsonAsync( "api/Administration/EditRole", rolesEditDto);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<RolesResponse>(content)!;
            }

            var errorResponse = JsonConvert.DeserializeObject<RolesResponse>(content);
            string apiMessage = !string.IsNullOrWhiteSpace(errorResponse?.Message) 
                ? errorResponse.Message 
                : "Failed to edit role";

            return new RolesResponse
            {
                StatusCode = (int)response.StatusCode,
                Message = apiMessage
            };
        }
        catch (Exception ex)
        {
            return new RolesResponse { StatusCode = 500, Message = ex.Message };
        }
    }
    
    // ==============================Users===============================================================
    
    
    public async Task<UserRS> ListUsers(UserVM userVm)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            var response = await _httpClient.PostAsJsonAsync( "api/Administration/GetListUsers", userVm);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<UserRS>(content)!;
                result.StatusCode = (int)response.StatusCode; 
                return result;
            }

            var errorResponse = JsonConvert.DeserializeObject<UserRS>(content);
            string apiMessage = !string.IsNullOrWhiteSpace(errorResponse?.Message) 
                ? errorResponse.Message 
                : "Failed to retrieve users list"; 

            return new UserRS
            {
                StatusCode = (int)response.StatusCode,
                Message = apiMessage
            };
        }
        catch (Exception ex)
        {
            return new UserRS 
            { 
                StatusCode = 500, 
                Message = $"Client Connection Error: {ex.Message}" 
            };
        }
    }

  
    public async Task<ClaimsRoelsUserRS> GetEditeUserClaims(string userId)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            

            var response = await _httpClient.GetAsync( $"api/Administration/GetUserClaimsAndRolesAsync/{userId}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ClaimsRoelsUserRS>(content)!;
            }

            var errorResponse = JsonConvert.DeserializeObject<ClaimsRoelsUserRS>(content);
            return errorResponse ?? new ClaimsRoelsUserRS
            { 
                StatusCode = (int)response.StatusCode,
                Message = "Failed to load user claims and roles."
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return new ClaimsRoelsUserRS
            {
                StatusCode = 500,
                Message = ex.Message
            };
        }
    }

    public async Task<ClaimsRoelsUserRS> EditeUserClaims(ClaimsRolesUserVM claimsRolesUserVM)
    {
        try
        {
            // ===============fro localStorage==============================
            //await _httpClient.AddBearerTokenAsync(_localStorage);
            
            
            var response = await _httpClient.PutAsJsonAsync( "api/Administration/UpdateUserClaimsAndRoles", claimsRolesUserVM);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ClaimsRoelsUserRS>(content)!;
            }

            var errorResponse = JsonConvert.DeserializeObject<ClaimsRoelsUserRS>(content);
            string apiMessage = !string.IsNullOrWhiteSpace(errorResponse?.Message) 
                ? errorResponse.Message 
                : "Failed to update user claims and roles";

            return new ClaimsRoelsUserRS
            {
                StatusCode = (int)response.StatusCode,
                Message = apiMessage
            };
        }
        catch (Exception ex)
        {
            return new ClaimsRoelsUserRS { StatusCode = 500, Message = ex.Message };
        }
    }
}
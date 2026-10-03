using System.Net.Http.Json;
using AppProject.Client.Authentication;
using AppProject.Client.Helpers;
using AppProject.Client.Models.Authentication;
using AppProject.Client.Models.Response.Account;
using AppProject.Client.Models.ViewModels.Account;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppProject.Client.Service.Account;

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AccountService(HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }


    //==================================Login=============================================


    public async Task<LoginRS?> LoginAsync(LoginVM loginVM)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync( "api/Account/Login", loginVM);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = JsonConvert.DeserializeObject<LoginRS>(content);
                try
                {
                    var userDto =
                        await _httpClient.GetFromJsonAsync<UserSessionDto>(
                                                                           "api/Account/IsUserAuth");
                    if (userDto != null && userDto.IsAuthenticated)
                    {
                        await ((CustomAuthStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(userDto);
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Authentication cookie failed to register on sequence call.");
                }

                return loginResponse;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<LoginRS>(content);
                return errorResponse ?? new LoginRS
                {
                    StatusCode = (int)response.StatusCode,
                    Message = response.ReasonPhrase
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login Exception: {ex.Message}");
            return new LoginRS
            {
                StatusCode = 500,
                Message = $"An error occurred while connecting to the server. {ex.Message}"
            };
        }
    }


    //==================================RegisterAsync=============================================

    public async Task<RegisterRS> RegisterAsync(RegisterVM registerVm)
    {
        try
        {
            var response =
                await _httpClient.PostAsJsonAsync("api/Account/Register", registerVm);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var registerRS = JsonConvert.DeserializeObject<RegisterRS>(content);
                return registerRS;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<RegisterRS>(content);
                return errorResponse ?? new RegisterRS
                {
                    StatusCode = (int)response.StatusCode,
                    Message = response.ReasonPhrase
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login Exception: {ex.Message}");
            return new RegisterRS
            {
                StatusCode = 500,
                Message = $"An error occurred while connecting to the server. {ex.Message}"
            };
        }
    }

    //==================================Active Account=============================================

    public async Task<bool> Active(ActiveAccountVM activeAccountVm)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync( "api/Account/active-account",
                activeAccountVm);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login Exception: {ex.Message}");
            return false;
        }
    }
    //==================================ForgotPasswordbyEmail=============================================

    public async Task<bool> ForgotPasswordbyEmail(string Email)
    {
        try
        {
            var response = await _httpClient.GetAsync( $"api/Account/send-email-forget-password?email={Email}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login Exception: {ex.Message}");
            return false;
        }
    }
    
    //==================================ResetPassword=============================================

    public async Task<ResetPasswordbyEmailRS> ResetPassword(RestPasswordVM restPasswordVm)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync( "api/Account/reset-password",restPasswordVm);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var resetPasswordbyEmailRS = JsonConvert.DeserializeObject<ResetPasswordbyEmailRS>(content);
                return resetPasswordbyEmailRS;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<ResetPasswordbyEmailRS>(content);
                return errorResponse;
                // return errorResponse ?? new ResetPasswordbyEmailRS
                // {
                //     StatusCode = (int)response.StatusCode,
                //     Message = errorResponse.Message
                // };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login Exception: {ex.Message}");
            return new ResetPasswordbyEmailRS
            {
                StatusCode = 500,
                Message = $"An error occurred while connecting to the server. {ex.Message}"
            };
        }
    }
    
    
    
}
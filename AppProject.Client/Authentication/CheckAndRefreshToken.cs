using System.Net.Http.Headers;
using System.Net.Http.Json;
using AppProject.Client.Helpers;
using AppProject.Client.Models.Authentication;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace AppProject.Client.Authentication;

public class CheckAndRefreshToken(
    HttpClient httpClient,
    ILocalStorageService localStorage,
    AuthenticationStateProvider authStateProvider)
{
    
    // ====================For Cookies===============================================================
   public async Task SetAuthorizeHeader()
{
    try
    { 
        var sessionState = await localStorage.GetItemAsync<UserSessionDto>("sessionState");
        
        if (sessionState != null && sessionState.IsAuthenticated)
        {
            if (sessionState.TokenExpired < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
                return;
            }
            if (sessionState.TokenExpired < DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds())
            {
                var refreshRes = await httpClient.GetAsync( "api/Account/loginByRefreshToken");
                if (!refreshRes.IsSuccessStatusCode)
                {
                    await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
                    return;
                }
            }
            var authCheckResponse = await httpClient.GetAsync( "api/Account/IsUserAuth");

            if (authCheckResponse.IsSuccessStatusCode)
            {
                var userDto = await authCheckResponse.Content.ReadFromJsonAsync<UserSessionDto>();
                if (userDto != null && userDto.IsAuthenticated)
                {
                    await ((CustomAuthStateProvider)authStateProvider).MarkUserAsAuthenticated(userDto);
                }
                else
                {
                    await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
                }
            }
        }
    }
    catch (Exception)
    {
        await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
    }
}
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AppProject.Client.Helpers;
using AppProject.Client.Models.Authentication;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace AppProject.Client.Authentication;

public class CustomAuthStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userDto = await localStorage.GetItemAsync<UserSessionDto>("sessionState");
            if (userDto == null)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            if (!userDto.IsAuthenticated)
            {
                userDto.TokenExpired = null;
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userDto.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, userDto.UserId ?? string.Empty)
            };

            if (userDto.Roles != null)
            {
                foreach (var role in userDto.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            
            if (userDto.Claims != null)
            {
                foreach (var item in userDto.Claims)
                {
                    claims.Add(new Claim(item.Key, item.Value));
                }
            }

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch (InvalidOperationException)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        catch (Exception ex)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }


    public async Task MarkUserAsAuthenticated(UserSessionDto userDto)
    {
        await localStorage.SetItemAsync("sessionState", userDto);
        var identity = await GetAuthenticationStateAsync();
        var user = new ClaimsPrincipal(identity.User);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
    
    
    public async Task MarkUserAsLoggedOut()
    {
        await localStorage.RemoveItemAsync("sessionState");
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}



//==================================for localStorage=============================================

// public class CustomAuthStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
// {
//     public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//     {
//         try
//         {
//             var sessionModel = await localStorage.GetItemAsync<UserSessionDto>("sessionState");
//             
//             if (sessionModel == null || string.IsNullOrEmpty(sessionModel.Token))
//             {
//                 return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//             }
//
//             var identity = GetClaimsIdentity(sessionModel.Token);
//             var user = new ClaimsPrincipal(identity);
//             return new AuthenticationState(user);
//         }
//         catch (InvalidOperationException)
//         {
//             return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//         }
//         catch (Exception)
//         {
//             return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//         }
//     }
//
//     public async Task MarkUserAsAuthenticated(UserSessionDto userDto)
//     {
//         await localStorage.SetItemAsync("sessionState", userDto);
//         var identity = GetClaimsIdentity(userDto.Token);
//         var user = new ClaimsPrincipal(identity);
//         NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
//     }
//
//     private ClaimsIdentity GetClaimsIdentity(string token)
//     {
//         try
//         {
//             var handler = new JwtSecurityTokenHandler();
//             var jwtToken = handler.ReadJwtToken(token);
//             var claims = jwtToken.Claims;
//             return new ClaimsIdentity(claims, "jwt");
//         }
//         catch
//         {
//             return new ClaimsIdentity();
//         }
//     }
//
//     public async Task MarkUserAsLoggedOut()
//     {
//         await localStorage.RemoveItemAsync("sessionState");
//         var identity = new ClaimsIdentity();
//         var user = new ClaimsPrincipal(identity);
//         NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
//     }
// }
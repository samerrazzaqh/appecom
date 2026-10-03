using System.Net.Http.Json;
using AppProject.Client.Helpers;
using AppProject.Client.Models.Authentication;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace AppProject.Client.Authentication;

public class CookieHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return base.SendAsync(request, cancellationToken);
    }
}



// public class CookieHandler : DelegatingHandler
// {
//     private readonly ILocalStorageService _localStorage;
//     private readonly AuthenticationStateProvider _authStateProvider;
//
//     public CookieHandler(ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
//     {
//         _localStorage = localStorage;
//         _authStateProvider = authStateProvider;
//     }
//
//     protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//     {
//         request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
//
//         // استثناء طلبات التحقق والتجديد لمنع التعليق والدوران اللانهائي
//         var path = request.RequestUri?.AbsolutePath ?? "";
//         if (!path.Contains("loginByRefreshToken") && !path.Contains("IsUserAuth"))
//         {
//             await ValidateAndRefreshTokenAsync(cancellationToken);
//         }
//
//         return await base.SendAsync(request, cancellationToken);
//     }
//
//     private async Task ValidateAndRefreshTokenAsync(CancellationToken cancellationToken)
//     {
//         try
//         {
//             var sessionState = await _localStorage.GetItemAsync<UserSessionDto>("sessionState");
//
//             if (sessionState != null && sessionState.IsAuthenticated)
//             {
//                 var nowSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
//
//                 // 1. التوكن منتهي
//                 if (sessionState.TokenExpired < nowSeconds)
//                 {
//                     await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
//                     return;
//                 }
//
//                 // إنشاء منفذ طلبات مستقل بدون Handlers لتجنب الدوران المالي
//                 using var innerHandler = new HttpClientHandler();
//                 using var rawClient = new HttpClient(innerHandler)
//                 {
//                     BaseAddress = new Uri(AppConstant.BaseApiAddress)
//                 };
//
//                 // 2. التوكن ينتهي خلال دقيقة -> تجديد
//                 if (sessionState.TokenExpired < DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds())
//                 {
//                     var refreshReq = new HttpRequestMessage(HttpMethod.Get, "api/Account/loginByRefreshToken");
//                     refreshReq.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
//                     
//                     var refreshRes = await rawClient.SendAsync(refreshReq, cancellationToken);
//                     if (!refreshRes.IsSuccessStatusCode)
//                     {
//                         await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
//                         return;
//                     }
//                 }
//
//                 // 3. التحقق من حالة التوثيق
//                 var authReq = new HttpRequestMessage(HttpMethod.Get, "api/Account/IsUserAuth");
//                 authReq.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
//
//                 var authCheckResponse = await rawClient.SendAsync(authReq, cancellationToken);
//                 if (authCheckResponse.IsSuccessStatusCode)
//                 {
//                     var userDto = await authCheckResponse.Content.ReadFromJsonAsync<UserSessionDto>(cancellationToken: cancellationToken);
//                     if (userDto != null && userDto.IsAuthenticated)
//                     {
//                         await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(userDto);
//                     }
//                     else
//                     {
//                         await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
//                     }
//                 }
//             }
//         }
//         catch
//         {
//             await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
//         }
//     }
// }
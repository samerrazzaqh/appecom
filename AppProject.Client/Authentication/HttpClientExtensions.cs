using System.Net.Http.Headers;
using AppProject.Client.Helpers;
using AppProject.Client.Models.Authentication;
using Blazored.LocalStorage;

namespace AppProject.Client.Authentication;

// ================ForCookieHandler===============================================
public static class HttpClientExtensions
{
    public static IHttpClientBuilder AddApiService<TClient, TImplementation>(this IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
    {
        return services.AddHttpClient<TClient, TImplementation>((sp, client) =>
            {
                // قراءة IConfiguration تلقائياً من الـ Dependency Injection Container
                var configuration = sp.GetRequiredService<IConfiguration>();
                 var apiBaseAddress = configuration["ApiSettings:BaseApiAddress"] ?? "http://localhost:5001/";
                // var apiBaseAddress = configuration["ApiSettings:BaseApiAddress"] ?? "https://samer-kubernetes.infra-x.cloud/";

                client.BaseAddress = new Uri(apiBaseAddress);
            })
            .AddHttpMessageHandler<CookieHandler>();
    }
    

// ================For localStorage===============================================
   
    // public static async Task AddBearerTokenAsync(this HttpClient httpClient, ILocalStorageService localStorage)
    // {
    //     var session = await localStorage.GetItemAsync<UserSessionDto>("sessionState");
    //
    //     if (session != null && !string.IsNullOrEmpty(session.Token))
    //     {
    //         httpClient.DefaultRequestHeaders.Authorization =
    //             new AuthenticationHeaderValue("Bearer", session.Token);
    //     }
    // }
    
    
    
}

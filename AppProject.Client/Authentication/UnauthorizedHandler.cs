using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace AppProject.Client.Authentication;

public class UnauthorizedHandler(AuthenticationStateProvider authStateProvider, NavigationManager navigationManager) 
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (authStateProvider is CustomAuthStateProvider customAuth)
            {
                await customAuth.MarkUserAsLoggedOut();
            }
            navigationManager.NavigateTo("/login");
        }
        return response;
    }
}
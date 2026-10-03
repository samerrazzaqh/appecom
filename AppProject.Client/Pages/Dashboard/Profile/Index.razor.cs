using AppProject.Client.Authentication;
using AppProject.Client.Models.Response.Profile;
using AppProject.Client.Service.Orders;
using AppProject.Client.Service.Profile;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace AppProject.Client.Pages.Dashboard.Profile;

public partial class Index : ComponentBase
{
    [Inject] public IProfileService? profileService { get; set; }
    public AddressRS addressRs { get; set; } = new AddressRS();
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public CheckAndRefreshToken CheckAndRefreshToken { get; set; }
    [Inject] private IOrderService? OrderService { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }

    public bool ShowMessage;
    public string? Message;
    private bool isVisible;


    protected override async Task OnInitializedAsync()
    {
        await CheckAndRefreshToken.SetAuthorizeHeader();
        
        var result = await profileService!.getUserInfoAddress();
        if (result != null)
        {
            addressRs.firstName = result.firstName;
            addressRs.lastName = result.lastName;
            addressRs.city = result.city;
            addressRs.zipCode = result.zipCode;
            addressRs.street = result.street;
            addressRs.state = result.state;
        }
        StateHasChanged();
    }

    private async Task GoEditeInfo()
    {
        Navigation.NavigateTo($"/editinfoprofile");
    }
    private async Task GoAllyourOrders()
    {
        Navigation.NavigateTo($"/AllOrdersForUser");
    }
    
}
using AppProject.Client.Authentication;
using AppProject.Client.Models.Response.Profile;
using AppProject.Client.Models.ViewModels.Profile;
using AppProject.Client.Service.Profile;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Profile;

public partial class EditInfoProfile : ComponentBase
{
    [Inject] public IProfileService? profileService { get; set; }

    public AddressRS? addressRs { get; set; } = new AddressRS();
    public ShipAddressVM? ShipAddressVM { get; set; } = new ShipAddressVM();

    [Inject] public NavigationManager Navigation { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }

    [Inject] public CheckAndRefreshToken CheckAndRefreshToken { get; set; }


    protected async override Task OnInitializedAsync()
    {
        var result = await profileService!.getUserInfoAddress();
        if (result != null)
        {
            addressRs.city = result.city;
            addressRs.firstName = result.firstName;
            addressRs.lastName = result.lastName;
            addressRs.state = result.state;
            addressRs.street = result.street;
            addressRs.zipCode = result.zipCode;
            StateHasChanged();
        }
    }


    protected async Task HandleValidSubmit()
    {
        ShipAddressVM.FirstName = addressRs.firstName;
        ShipAddressVM.LastName = addressRs.lastName;
        ShipAddressVM.City = addressRs.city;
        ShipAddressVM.ZipCode = addressRs.zipCode;
        ShipAddressVM.Street = addressRs.street;
        ShipAddressVM.State = addressRs.state;
        var result = await profileService!.UpdateAddress(ShipAddressVM!);

        if (result)
        {
            Navigation!.NavigateTo("javascript:history.back()");
        }
        else
        {
            Snackbar.Add($"The information has not been updated; there is an error.", Severity.Error);
        }
    }
}
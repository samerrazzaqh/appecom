using AppProject.Client.Models.ViewModels.Account;
using AppProject.Client.Service.Account;
using Microsoft.AspNetCore.Components;

namespace AppProject.Client.Pages.Dashboard.Account;

public partial class ResetPasswordbyEmail : ComponentBase
{
    
    [SupplyParameterFromQuery]
    public string? Email { get; set; }

    [SupplyParameterFromQuery]
    public string? Code { get; set; }
    [Inject]
    public IAccountService? accountService { get; set; }
    
    [Inject]
    public NavigationManager? NavigationManager { get; set; }
    
    public RestPasswordVM restPasswordVM = new RestPasswordVM();
    
    public bool ShowMessage;
    public string? Message;


    
    public async Task HandleResetPasswordbyEmail()
    {
        ShowMessage = false;
        if (!string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Email) )
        {
            restPasswordVM.Token = Uri.UnescapeDataString(Code);
            restPasswordVM.Email = Email;
            Console.WriteLine("============"+ restPasswordVM.Token);
        }
        var result = await accountService!.ResetPassword(restPasswordVM);

        if (result!.StatusCode == 200)
        {

            NavigationManager!.NavigateTo("/login");
        }
        else
        {
            Message = result.Message;
            ShowMessage = true;
        }
        StateHasChanged();
    }
    
    
}
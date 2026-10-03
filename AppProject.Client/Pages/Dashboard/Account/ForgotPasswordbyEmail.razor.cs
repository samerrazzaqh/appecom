using AppProject.Client.Service.Account;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Account;

public partial class ForgotPasswordbyEmail:ComponentBase
{
    [Inject]
    public IAccountService? accountService { get; set; }
    
    public string? email;
    public bool ShowMessage;
    public string? Message;
    private bool isVisiblebtn;
    [Inject]
    public NavigationManager? NavigationManager { get; set; }
    
    [Inject]
    public ISnackbar Snackbar { get; set; }

    public async Task HandleForgotPasswordbyEmail()
    {
        ShowMessage = false;
        isVisiblebtn = true;
        var result = await accountService!.ForgotPasswordbyEmail(email);

        if (result == true)
        {
            Snackbar.Add($"Check your Email: {email}", Severity.Info);
        }
        else
        {
            Message = "Email Not Found";
            ShowMessage = true;
        }
        StateHasChanged();
        isVisiblebtn = false;
    }
}
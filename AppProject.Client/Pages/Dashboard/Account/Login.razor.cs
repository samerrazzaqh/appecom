using AppProject.Client.Models.ViewModels.Account;
using AppProject.Client.Service.Account;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Account;

public partial class Login : ComponentBase
{
    [SupplyParameterFromQuery(Name = "returnUrl")]
    public string? ReturnUrl { get; set; }


    [SupplyParameterFromQuery(Name = "messageactive")]
    public string? messageActive { get; set; }

    [SupplyParameterFromQuery(Name = "success")]
    public bool SuccessActive { get; set; }

    [Inject] public ISnackbar Snackbar { get; set; }

    [Inject] public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] public IAccountService? accountService { get; set; } = default!;
    private LoginVM loginVM = new LoginVM();
    public bool ShowMessage;
    public string? Message;
    private bool isVisiblebtn;
    [Inject] public NavigationManager? NavigationManager { get; set; } = default!;

    public async Task HandleLogin()
    {
        ShowMessage = false;
        isVisiblebtn = true;

        var result = await accountService!.LoginAsync(loginVM);

        if (result != null && result.StatusCode == 200)
        {
            NavigationManager.NavigateTo(string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl);
        }
        else
        {
            Message = result?.Message;
            ShowMessage = true;
        }

        StateHasChanged();
        isVisiblebtn = false;
    }


    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider!.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity!.IsAuthenticated)
        {
            NavigationManager!.NavigateTo(string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl);
        }

        if (messageActive is not null)
        {
            if (SuccessActive)
            {
                Snackbar.Add($"{messageActive}", Severity.Success);
            }
            else
            {
                Snackbar.Add($"{messageActive}", Severity.Error);
            }
        }

        StateHasChanged();
    }

    bool isShow;
    InputType PasswordInput = InputType.Password;
    string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    public void ButtonTestclick()
    {
        if (isShow)
        {
            isShow = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInput = InputType.Password;
        }
        else
        {
            isShow = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInput = InputType.Text;
        }
    }
}
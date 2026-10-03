using AppProject.Client.Models.ViewModels.Account;
using AppProject.Client.Service.Account;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Account;

public partial class Register : ComponentBase
{
    [Inject]
    public IAccountService? accountService { get; set; }
    public RegisterVM registerVM= new RegisterVM();
    public bool ShowMessage;
    public bool Success;
    public string? Message;

    private bool isVisiblebtn;

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    private async Task HandleRegistration()
    {
        isVisiblebtn = true;
        ShowMessage = false;

        var result = await accountService!.RegisterAsync(registerVM);

        if (result!.StatusCode == 200)
        {
            // //===============Encode==================================
            // var UTF8 = System.Text.Encoding.UTF8.GetBytes(result.Email!);
            // var ToBase64String_Email = Convert.ToBase64String(UTF8);
            //=================================================
            NavigationManager!.NavigateTo($"/login",forceLoad:true);
        }
        else
        {
            Message = result.Message;
            ShowMessage = true;
        }
        StateHasChanged();
        isVisiblebtn = false;
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
using AppProject.Client.Models.ViewModels.Administration;
using AppProject.Client.Service.Administration;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Administration.Role;

public partial class DialogCreateRole:ComponentBase
{
    [Inject]
    public IAdministrationService? AdministrationService { get; set; }
    public RolesVM RolesVm { get; set; } = new();
    public bool ShowMessage;
    public string? Message;
    [Inject] public ISnackbar? Snackbar { get; set; }
    [Parameter]
    public EventCallback OnCreateCategory { get; set; }

    [Inject]
    public NavigationManager? NavigationManager { get; set; }


    public async Task HandleCreate()
    {
        ShowMessage = false;

        var result = await AdministrationService!.CreateRole(RolesVm);

        if (result != null && result.StatusCode == 200)
        {
            Snackbar?.Add("Role created successfully", Severity.Success);
            Submit();
        }
        else
        {
            Snackbar?.Add($"{result?.Message ?? "Error occurred during create"}", Severity.Error);
        }
        StateHasChanged();
    }


    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }

    public void Submit()
    {
        MudDialog!.Close(DialogResult.Ok(true));

    }
    public void Cancel() => MudDialog!.Cancel();

}
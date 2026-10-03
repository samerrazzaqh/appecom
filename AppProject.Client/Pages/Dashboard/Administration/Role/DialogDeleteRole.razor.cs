using AppProject.Client.Service.Administration;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Administration.Role;

public partial class DialogDeleteRole:ComponentBase
{

    [Inject]
    public IAdministrationService? AdministrationService { get; set; }

    [Inject] public ISnackbar? Snackbar { get; set; }
    [Parameter]
    public string? roleId { get; set; }

    public async Task HandleDelete()
    {
        var result =  await AdministrationService!.DeleteRole(roleId);
        
        if (result != null && result.StatusCode == 200)
        {
            Snackbar?.Add("Role delete successfully", Severity.Success);
        }
        else
        {
            Snackbar?.Add($"{result?.Message ?? "Error occurred during delete"}", Severity.Error);
        }
    }


    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }

    public async Task Submit()
    {
         await HandleDelete();
         MudDialog!.Close(DialogResult.Ok(true));
         StateHasChanged();
    }
    public void Cancel() => MudDialog!.Cancel();


}
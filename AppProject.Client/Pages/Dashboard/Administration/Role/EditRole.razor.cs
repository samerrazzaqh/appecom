using AppProject.Client.Models.Response.Administration;
using AppProject.Client.Models.ViewModels.Administration;
using AppProject.Client.Service.Administration;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Administration.Role;

public partial class EditRole : ComponentBase
{
    [Parameter] public string? RoleId { get; set; }
    [Parameter] public string? RoleName { get; set; }
    
    [Inject] public ISnackbar? Snackbar { get; set; }
    [Inject] public IAdministrationService? AdministrationService { get; set; }
    [Inject] public NavigationManager? NavigationManager { get; set; }

    private MudTable<UserRoleRS>? _table;
    public RolesVM RolesVm { get; set; } = new();
    public RolesResponse<DataRoles> RolesResponse { get; set; } = new();
    public List<UserRoleRS> UsersList { get; set; } = new();
    public string? _searchString;

    protected override void OnInitialized()
    {
        RolesVm.RoleId = RoleId ?? "";
        RolesVm.RoleName = RoleName ?? "";
    }

    private async Task<TableData<UserRoleRS>> ServerReload(TableState state, CancellationToken token)
    {
        RolesVm.PageNumber = state.Page + 1; 
        RolesVm.PageSize = state.PageSize;  
        RolesVm.SearchTerm = _searchString ?? "";

        RolesResponse = await AdministrationService!.GetUsersInRole(RolesVm);

        if (RolesResponse?.Data?.UserRoleRS != null)
        {
            UsersList = RolesResponse.Data.UserRoleRS;

            return new TableData<UserRoleRS>()
            {
                TotalItems = RolesResponse.Data.TotalCount,
                Items = UsersList
            };
        }

        return new TableData<UserRoleRS>() { TotalItems = 0, Items = new List<UserRoleRS>() };
    }

    private async Task OnSearch(string text)
    {
        _searchString = text;
        if (_table != null)
        {
            await _table.ReloadServerData();
        }
    }

    public async Task HandleEditRole()
    {
        var rolesEditVm = new RolesEditVM
        {
            RoleId = RolesVm.RoleId,
            RoleName = RolesVm.RoleName,
            UserRole = UsersList
        };

        var result = await AdministrationService!.EditRole(rolesEditVm);

        if (result != null && result.StatusCode == 200)
        {
            Snackbar?.Add("Role updated successfully", Severity.Success);
            if (_table != null)
            {
                await _table.ReloadServerData();
            }
        }
        else
        {
            Snackbar?.Add($"{result?.Message ?? "Error occurred during edit"}", Severity.Error);
        }
    }
}
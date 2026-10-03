using AppProject.Client.Models.Response.Administration;
using AppProject.Client.Service.Administration;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Administration.Role;

public partial class ListRoles: ComponentBase
	{
        public IEnumerable<RolesRS> rolesRS = new List<RolesRS>();

        [Inject]
        public NavigationManager? NavigationManager { get; set; }
        
        [Inject]
        public IAdministrationService? AdministrationService { get; set; }
        //==================================================================
        private bool isVisible;



        protected override async Task OnInitializedAsync()
        {
            isVisible = true;
            rolesRS = (await AdministrationService!.ListRoles())!.ToList();
            isVisible = false;
            StateHasChanged();
        }

        public bool _sortNameByLength;
        public SortMode _sortMode = SortMode.Multiple;

        //// custom sort by name length
        private Func<RolesRS, object> _sortBy => x =>
        {
            if (_sortNameByLength)
                return x.RoleId!;
            else
                return x.RoleName!;
        };


        public string? _searchString;
        public Func<RolesRS, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.RoleName!.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };




        [Inject]
        public IDialogService? DialogService { get; set; }
        DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

        public async Task OpenDialogCreate(DialogOptions options)
        {
            var dialog = await DialogService!.ShowAsync<DialogCreateRole>("Add New Role", options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                rolesRS = (await AdministrationService!.ListRoles())!.ToList();
            }
        }

        public void OpenDialogEdite(RolesRS model)
        {
            NavigationManager!.NavigateTo($"/editrole/{model.RoleId}/{model.RoleName}");
        }


        public async Task OpenDialogDelete(RolesRS model)
        {
            var parameters = new DialogParameters();
            parameters.Add("roleId", model.RoleId);
            var dialog = await DialogService!.ShowAsync<DialogDeleteRole>($"Are you Delete {model.RoleName}", parameters);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                rolesRS = (await AdministrationService!.ListRoles())!.ToList();
            }
        }





    }
using AppProject.Client.Models.Response.Administration;
using AppProject.Client.Models.ViewModels.Administration;
using AppProject.Client.Service.Administration;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Administration.User;

public partial class EditeUserClaims : ComponentBase
    {
        [Parameter] public string UserId { get; set; } = string.Empty;

        [Inject] public IAdministrationService AdministrationService { get; set; } = null!;
        [Inject] public ISnackbar Snackbar { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;

        public DataClaimsRoelsUser UserData { get; set; } = new();
        public bool IsLoading { get; set; } = true;
        public bool IsSubmitting { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadUserData();
        }

        private async Task LoadUserData()
        {
            IsLoading = true;
            var response = await AdministrationService.GetEditeUserClaims(UserId);

            if (response != null && response.StatusCode == 200 && response.DataClaimsRoelsUser != null)
            {
                UserData = response.DataClaimsRoelsUser;
            }
            else
            {
                Snackbar.Add(response?.Message ?? "Error loading data", Severity.Error);
            }

            IsLoading = false;
        }

        private async Task HandleEditRoleandClaims()
        {
            IsSubmitting = true;

            // 1. Map DataClaimsRoelsUser to ClaimsRolesUserVM
            var updateModel = new ClaimsRolesUserVM
            {
                UserId = UserData.Id ?? UserId,
                Block = UserData.Block,
                Claims = UserData.ClaimsUser?.Select(c => new Claims
                {
                    ClaimType = c.ClaimType,
                    IsSelected = c.IsSelected
                }).ToList() ?? new List<Claims>(),
                Roles = UserData.RolesUser?.Select(r => new Roles
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    IsSelected = r.IsSelected
                }).ToList() ?? new List<Roles>()
            };

            // 2. Send Update Request
            var result = await AdministrationService.EditeUserClaims(updateModel);

            if (result != null && result.StatusCode == 200)
            {
                Snackbar.Add(result.Message ?? "Data updated successfully!", Severity.Success);
                NavigationManager!.NavigateTo("javascript:history.back()");
            }
            else
            {
                Snackbar.Add(result?.Message ?? "Failed to update user data", Severity.Error);
            }

            IsSubmitting = false;
        }
    }
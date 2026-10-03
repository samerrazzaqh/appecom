using AppProject.Client.Models.Response.Administration;
using AppProject.Client.Models.ViewModels.Administration;
using AppProject.Client.Service.Administration;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Administration.User;

public partial class ListUsers : ComponentBase
    {
        [Inject] public ISnackbar? Snackbar { get; set; }
        [Inject] public IAdministrationService? AdministrationService { get; set; }
        [Inject] public NavigationManager? NavigationManager { get; set; }

        private MudTable<UserResponse> table = null!;
        private string searchString = string.Empty;
        private bool isLoading = false;
        private int currentPage = 0;
        private int pageSize = 5;

        private async Task<TableData<UserResponse>> LoadServerData(TableState state, CancellationToken token)
        {
            isLoading = true;
            StateHasChanged();

            currentPage = state.Page;
            pageSize = state.PageSize;

            var request = new UserVM
            {
                SearchTerm = searchString,
                PageNumber = state.Page + 1,
                PageSize = state.PageSize
            };

            try
            {
                var result = await AdministrationService!.ListUsers(request);

                if (result != null && result.StatusCode == 200 && result.UserResponses != null)
                {
                    return new TableData<UserResponse>
                    {
                        TotalItems = result.TotalCount,
                        Items = result.UserResponses
                    };
                }

                Snackbar?.Add(result?.Message ?? "Failed to load users", Severity.Error);
                return new TableData<UserResponse> { TotalItems = 0, Items = new List<UserResponse>() };
            }
            catch (Exception ex)
            {
                Snackbar?.Add($"Error: {ex.Message}", Severity.Error);
                return new TableData<UserResponse> { TotalItems = 0, Items = new List<UserResponse>() };
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private int GetRowIndex(UserResponse user)
        {
            var items = table.FilteredItems.ToList();
            return (currentPage * pageSize) + items.IndexOf(user) + 1;
        }

        private void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }

        private void OnEditUser(UserResponse user)
        {
            // التنقل إلى صفحة تعديل المستخدم بالـ UserId
            NavigationManager?.NavigateTo($"/EditeUserClaims/{user.UserId}");
        }

        private async Task OnDeleteUser(UserResponse user)
        {
            // إمكانية إضافة حوار تأكيد الحذف (MudMessageBox) هنا مستقبلاً
            Snackbar?.Add($"Deleting user {user.UserName}...", Severity.Warning);
        }
    }
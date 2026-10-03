using AppProject.Client.Authentication;
using AppProject.Client.Models.Response.Orders;
using AppProject.Client.Models.ViewModels.Rating;
using AppProject.Client.Pages.Component;
using AppProject.Client.Service.Orders;
using AppProject.Client.Service.Rating;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Public;

public partial class AllOrdersForUser : ComponentBase
{
    [Inject] private IOrderService? OrderService { get; set; }
    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] public CheckAndRefreshToken CheckAndRefreshToken { get; set; } = default!;

    public List<OrderRS> Orders { get; set; } = new();
    public bool IsLoading { get; set; } = true;
    public string SearchTerm { get; set; } = string.Empty;

    public Status? SelectedStatusFilter { get; set; } = null;

    public enum Status { Pending, PaymentReceived, PaymentFaild }

    public IEnumerable<OrderRS> FilteredOrders => Orders
        .Where(o => !SelectedStatusFilter.HasValue || 
                    o.Status.Equals(SelectedStatusFilter.Value.ToString(), StringComparison.OrdinalIgnoreCase))
        .Where(o => string.IsNullOrWhiteSpace(SearchTerm) ||
                    o.Id.ToString().Contains(SearchTerm) ||
                    o.Status.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    o.OrderItems.Any(i => i.ProductName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)));

    protected override async Task OnInitializedAsync()
    {
        if (CheckAndRefreshToken != null)
        {
            await CheckAndRefreshToken.SetAuthorizeHeader();
        }

        if (OrderService != null)
        {
            var result = await OrderService.GetAllOrdersForUserAsync();
            Orders = result?.OrderByDescending(o => o.OrderDate).ToList() ?? new List<OrderRS>();
        }

        IsLoading = false;
    }

    private void SetStatusFilter(Status? status)
    {
        SelectedStatusFilter = status;
    }

    private Color GetStatusColor(string statusStr)
    {
        if (Enum.TryParse<Status>(statusStr, true, out var status))
        {
            return status switch
            {
                Status.Pending => Color.Warning,
                Status.PaymentReceived => Color.Success,
                Status.PaymentFaild => Color.Error,
                _ => Color.Info
            };
        }
        return Color.Info;
    }

    private void ViewOrderDetails(int orderId)
    {
        NavigationManager.NavigateTo($"/ViewOrder/{orderId}");
    }

    private void GoToHome()
    {
        NavigationManager.NavigateTo("/");
    }

    private async Task OpenRatingDialogAsync(int productId, string productName)
    {
        var parameters = new DialogParameters<RatingDialog>
        {
            { x => x.ProductId, productId },
            { x => x.ProductName, productName }
        };

        var options = new DialogOptions 
        { 
            CloseButton = true, 
            MaxWidth = MaxWidth.ExtraSmall, 
            FullWidth = true 
        };

        await DialogService.ShowAsync<RatingDialog>("Rate Product", parameters, options);
    }
}
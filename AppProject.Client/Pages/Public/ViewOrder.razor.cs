using AppProject.Client.Authentication;
using AppProject.Client.Models.Response.Orders;
using AppProject.Client.Service.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace AppProject.Client.Pages.Public;

public partial class ViewOrder : ComponentBase
{
    [Parameter] public int OrderId { get; set; }
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IOrderService? OrderService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] public CheckAndRefreshToken CheckAndRefreshToken { get; set; } = default!;
    public enum Status { Pending, PaymentReceived, PaymentFaild }
    public OrderRS? Order { get; set; }
    public bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        if (CheckAndRefreshToken != null)
        {
            await CheckAndRefreshToken.SetAuthorizeHeader();
        }

        if (OrderId > 0 && OrderService != null)
        {
            Order = await OrderService.GetOrderByIdAsync(OrderId);
        }

        IsLoading = false;
    }
    private async Task ExportToPdf()
    {
        // استدلاء نافذة الطباعة الخاصة بالمتصفح وتحويلها لـ PDF
        await JS.InvokeVoidAsync("window.print");
    }

    private void GoBackToHome()
    {
        NavigationManager.NavigateTo("/");
    }

    private Color GetStatusColor(string status) => status switch
    {
        nameof(Status.Pending) => Color.Warning,
        nameof(Status.PaymentReceived) => Color.Success,
        nameof(Status.PaymentFaild) => Color.Error,
        _ => Color.Info
    };
}
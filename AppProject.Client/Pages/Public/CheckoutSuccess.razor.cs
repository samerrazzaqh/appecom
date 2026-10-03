using AppProject.Client.Authentication;
using AppProject.Client.Models.Response.Orders;
using AppProject.Client.Models.ViewModels.Orders;
using AppProject.Client.Service.Basket;
using AppProject.Client.Service.Orders;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Public;

public partial class CheckoutSuccess : ComponentBase
{
    [Parameter] public string SessionId { get; set; } = string.Empty;

    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] public CheckAndRefreshToken CheckAndRefreshToken { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject] private IBasketService BasketService { get; set; } = default!;
    [Inject] private BasketStateService BasketState { get; set; } = default!;

    private bool isLoading = true;
    private int createdOrderId;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (CheckAndRefreshToken != null)
            {
                await CheckAndRefreshToken.SetAuthorizeHeader();
            }

            // 👈 1. قراءة رقم الطلب المحفوظ من LocalStorage
            createdOrderId = await LocalStorage.GetItemAsync<int>("last_order_id");

            // 2. تفريغ السلة
            await ClearFullBasket();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing checkout success: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task ClearFullBasket()
    {
        try
        {
            string targetBasketId = await LocalStorage.GetItemAsync<string>("basket_id");

            if (!string.IsNullOrEmpty(targetBasketId) && BasketService != null)
            {
                await BasketService.DeleteBasketAsync(targetBasketId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Basket already cleared: {ex.Message}");
        }
        finally
        {
            // تنظيف السلة والرقم المؤقت للطلب
            await LocalStorage.RemoveItemAsync("basket_id");
            await LocalStorage.RemoveItemAsync("last_order_id");

            if (BasketState != null)
            {
                await BasketState.UpdateBasketCountAsync();
            }

            StateHasChanged();
        }
    }

    private void NavigateToOrders()
    {
        // 👈 التوجيه لصفحة عرض الطلب المباشرة
        if (createdOrderId > 0)
        {
            NavigationManager.NavigateTo($"/ViewOrder/{createdOrderId}");
        }
        else
        {
            NavigationManager.NavigateTo("/");
        }
    }

    private void NavigateToHome()
    {
        NavigationManager.NavigateTo("/");
    }
}
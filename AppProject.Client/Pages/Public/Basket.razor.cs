using AppProject.Client.Models.ViewModels.Basket;
using AppProject.Client.Service.Basket;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Public;

public partial class Basket : ComponentBase
    {
        [Inject] public ISnackbar Snackbar { get; set; } = default!;
        [Inject] private IBasketService BasketService { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private BasketStateService BasketState { get; set; } = default!;

        private CustomerBasketVM? basket;
        private bool isLoading = true;

        private decimal Subtotal => basket?.BasketItems?.Sum(x => x.Price * x.Qunatity) ?? 0;
        
        // تعديل قيمة الشحن لتصبح مجانية ($0.00)
        private decimal Shipping => 0.00m; 
        private decimal TotalPrice => Subtotal + Shipping;

        protected override async Task OnInitializedAsync()
        {
            await LoadBasket();
        }

        private async Task LoadBasket()
        {
            isLoading = true;
            try
            {
                string basketId = await LocalStorage.GetItemAsync<string>("basket_id");
                if (!string.IsNullOrEmpty(basketId))
                {
                    var basketRS = await BasketService.GetBasketAsync(basketId);
                    if (basketRS != null)
                    {
                        basket = new CustomerBasketVM
                        {
                            Id = basketRS.Id,
                            PaymentIntentId = basketRS.PaymentIntentId ?? "string",
                            ClientSecret = basketRS.ClientSecret ?? "string",
                            BasketItems = basketRS.BasketItems?.Select(i => new BasketItems
                            {
                                Id = i.Id,
                                Name = i.Name,
                                Description = i.Description,
                                Image = i.Image,
                                Price = i.Price,
                                Qunatity = i.Quantity,
                                Category = i.Category
                            }).ToList() ?? new List<BasketItems>()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading basket: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task UpdateQuantity(BasketItems item, int newQty)
        {
            if (newQty <= 0)
            {
                await RemoveItem(item);
                return;
            }

            item.Qunatity = newQty;
            await SaveBasketChanges();
        }

        private async Task RemoveItem(BasketItems item)
        {
            if (basket == null) return;

            basket.BasketItems.Remove(item);

            if (!basket.BasketItems.Any())
            {
                await ClearFullBasket();
            }
            else
            {
                await SaveBasketChanges();
                Snackbar.Add("Item removed from basket", Severity.Info);
            }
        }

        private async Task ClearFullBasket()
        {
            if (basket == null) return;

            bool isDeleted = await BasketService.DeleteBasketAsync(basket.Id);

            if (isDeleted)
            {
                await LocalStorage.RemoveItemAsync("basket_id");
                basket = null;
                await BasketState.UpdateBasketCountAsync();
                Snackbar.Add("Basket cleared successfully", Severity.Success);
            }
            else
            {
                Snackbar.Add("Failed to clear basket", Severity.Error);
            }

            StateHasChanged();
        }

        private async Task SaveBasketChanges()
        {
            if (basket == null) return;

            var result = await BasketService.AddOrUpdateBasketAsync(basket);
            if (result != null && result.StatusCode != 400)
            {
                await BasketState.UpdateBasketCountAsync();
                StateHasChanged();
            }
            else
            {
                Snackbar.Add("Failed to update basket", Severity.Error);
            }
        }

        private void NavigateToProductDetails(int productId, int itemQuantity)
        {
            NavigationManager.NavigateTo($"/ProductDetails/{productId}?quantity={itemQuantity}");
        }

        private void NavigateToShopping()
        {
            NavigationManager.NavigateTo("/shopping");
        }

        // دالة التوجيه لصفحة الدفع Checkout
        private void NavigateToCheckout()
        {
            if (basket == null || !basket.BasketItems.Any())
            {
                Snackbar.Add("Your basket is empty!", Severity.Warning);
                return;
            }

            NavigationManager.NavigateTo("/Checkout");
        }
    }
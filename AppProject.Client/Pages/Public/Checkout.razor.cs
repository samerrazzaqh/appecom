using AppProject.Client.Authentication;
using AppProject.Client.Models.Response.Orders;
using AppProject.Client.Models.Response.Profile;
using AppProject.Client.Models.ViewModels.Basket;
using AppProject.Client.Models.ViewModels.Orders;
using AppProject.Client.Service.Basket;
using AppProject.Client.Service.Orders;
using AppProject.Client.Service.Payment;
using AppProject.Client.Service.Profile;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Public;

public partial class Checkout : ComponentBase
{
    [Inject] private IBasketService BasketService { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] public IOrderService OrderService { get; set; } = default!;
    [Inject] public IProfileService? ProfileService { get; set; }
    [Inject] public IPaymentService? PaymentService { get; set; }
    [Inject] private BasketStateService BasketState { get; set; } = default!;
    [Inject] public CheckAndRefreshToken CheckAndRefreshToken { get; set; } = default!;

    private int _activeIndex = 0;
    private bool isLoading = true;
    int createdOrderId = 0;

    private CustomerBasketVM? basket;
    public AddressRS addressRs { get; set; } = new AddressRS();

    // خيارات الشحن
    private IEnumerable<DeliveryRS> deliveryMethods = new List<DeliveryRS>();
    private int _selectedDeliveryMethodId;
    private decimal selectedShippingPrice = 0.00m;

    private decimal Subtotal => basket?.BasketItems?.Sum(x => x.Price * x.Qunatity) ?? 0;
    private decimal TotalPrice => Subtotal + selectedShippingPrice;

    private bool IsAddressValid =>
        !string.IsNullOrWhiteSpace(addressRs.firstName) &&
        !string.IsNullOrWhiteSpace(addressRs.lastName) &&
        !string.IsNullOrWhiteSpace(addressRs.street) &&
        !string.IsNullOrWhiteSpace(addressRs.city) &&
        !string.IsNullOrWhiteSpace(addressRs.state) &&
        !string.IsNullOrWhiteSpace(addressRs.zipCode);

    protected override async Task OnInitializedAsync()
    {
        await CheckAndRefreshToken.SetAuthorizeHeader();
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        try
        {
            // 1. جلب السلة
            string basketId = await LocalStorage.GetItemAsync<string>("basket_id");
            if (!string.IsNullOrEmpty(basketId))
            {
                var basketRS = await BasketService.GetBasketAsync(basketId);
                if (basketRS != null && basketRS.BasketItems?.Any() == true)
                {
                    basket = new CustomerBasketVM
                    {
                        Id = basketRS.Id,
                        // تغيير "string" إلى null كي يتعرف عليها Stripe كـ PaymentIntent جديد
                        PaymentIntentId =
                            (string.IsNullOrEmpty(basketRS.PaymentIntentId) || basketRS.PaymentIntentId == "string")
                                ? null
                                : basketRS.PaymentIntentId,
                        ClientSecret =
                            (string.IsNullOrEmpty(basketRS.ClientSecret) || basketRS.ClientSecret == "string")
                                ? null
                                : basketRS.ClientSecret,
                        BasketItems = basketRS.BasketItems.Select(i => new BasketItems
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Description = i.Description,
                            Image = i.Image,
                            Price = i.Price,
                            Qunatity = i.Quantity,
                            Category = i.Category
                        }).ToList()
                    };
                }
                else
                {
                    NavigationManager.NavigateTo("/Basket");
                    return;
                }
            }
            else
            {
                NavigationManager.NavigateTo("/Basket");
                return;
            }

            // 2. جلب طرق الشحن من OrderService
            if (OrderService != null)
            {
                deliveryMethods = await OrderService.GetDeliveryMethodAsync();
                if (deliveryMethods.Any())
                {
                    var firstMethod = deliveryMethods.First();
                    selectedDeliveryMethodId = firstMethod.Id;
                    selectedShippingPrice = firstMethod.Price;
                }
            }

            // 3. جلب بيانات العنوان
            if (ProfileService != null)
            {
                var userAddress = await ProfileService.getUserInfoAddress();
                if (userAddress != null)
                {
                    addressRs = userAddress;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading checkout data: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
    }

    public int selectedDeliveryMethodId
    {
        get => _selectedDeliveryMethodId;
        set
        {
            if (_selectedDeliveryMethodId != value)
            {
                _selectedDeliveryMethodId = value;

                var selectedMethod = deliveryMethods.FirstOrDefault(d => d.Id == value);
                if (selectedMethod != null)
                {
                    selectedShippingPrice = selectedMethod.Price;
                }
            }
        }
    }


    private async Task HandlePayment()
    {
        if (!IsAddressValid)
        {
            Snackbar.Add("Please fill in all address details before payment.", Severity.Warning);
            _activeIndex = 0;
            return;
        }

        try
        {
            isLoading = true;
            string basketId = await LocalStorage.GetItemAsync<string>("basket_id");

            if (string.IsNullOrEmpty(basketId))
            {
                Snackbar.Add("Basket session expired or not found.", Severity.Error);
                return;
            }

            // 1. توليد وتحديث الـ PaymentIntent في السلة أولاً للحصول على pi_... الحقيقي بدلاً من "string"
            if (PaymentService != null)
            {
                var updatedBasket = await PaymentService.CreateOrUpdatePaymentAsync(basketId, selectedDeliveryMethodId);

                if (updatedBasket == null || string.IsNullOrEmpty(updatedBasket.PaymentIntentId) ||
                    updatedBasket.PaymentIntentId == "string")
                {
                    Snackbar.Add("Failed to generate payment intent with Stripe.", Severity.Error);
                    return;
                }

                // تحديث كائن السلة المحلي بالـ PaymentIntentId الحقيقي
                basket.PaymentIntentId = updatedBasket.PaymentIntentId;
            }

            // 2. تجهيز بيانات الطلب OrderVM
            var orderVm = new OrderVM
            {
                DeliveryMethodId = selectedDeliveryMethodId,
                BasketId = basketId,
                ShipAddress = new ShipAddress
                {
                    FirstName = addressRs.firstName,
                    LastName = addressRs.lastName,
                    Street = addressRs.street,
                    City = addressRs.city,
                    State = addressRs.state,
                    ZipCode = addressRs.zipCode
                }
            };

            // 3. إنشاء الطلب في قاعدة البيانات وبداخله الـ PaymentIntentId الحقيقي
            if (OrderService != null)
            {
                var createdOrder = await OrderService.CreateOrdersAsync(orderVm);

                if (createdOrder != null)
                {
                    createdOrderId = createdOrder.Id; // حفظ المعرف
                    await LocalStorage.SetItemAsync("last_order_id", createdOrder.Id);
                }
                else
                {
                    Snackbar.Add("Failed to create order. Please try again.", Severity.Error);
                    return;
                }
            }

            // 4. إنشاء جلسة Checkout Session من Stripe والتوجيه لصفحة الدفع
            if (PaymentService != null)
            {
                string? checkoutUrl =
                    await PaymentService.CreateCheckoutSessionAsync(basketId, selectedDeliveryMethodId,createdOrderId);

                if (!string.IsNullOrEmpty(checkoutUrl))
                {
                    NavigationManager.NavigateTo(checkoutUrl, forceLoad: true);
                }
                else
                {
                    Snackbar.Add("Failed to initiate Stripe Checkout Session.", Severity.Error);
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"An error occurred: {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoading = false;
        }
    }
}
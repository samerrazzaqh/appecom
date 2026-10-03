using Blazored.LocalStorage;

namespace AppProject.Client.Service.Basket;

public class BasketStateService
{
    private readonly IBasketService _basketService;
    private readonly ILocalStorageService _localStorage;

    public int BasketCount { get; private set; } = 0;
    
    // حدث يتم إطلاقه عند تغيير عدد عناصر السلة
    public event Action? OnChange;

    public BasketStateService(IBasketService basketService, ILocalStorageService localStorage)
    {
        _basketService = basketService;
        _localStorage = localStorage;
    }

    // تحديث العدد وجلب البيانات من الـ API
    public async Task UpdateBasketCountAsync()
    {
        try
        {
            string basketId = await _localStorage.GetItemAsync<string>("basket_id");
            if (!string.IsNullOrEmpty(basketId))
            {
                var basket = await _basketService.GetBasketAsync(basketId);
                if (basket?.BasketItems != null)
                {
                    // حساب مجموع الكميات للمنتجات في السلة
                    BasketCount = basket.BasketItems.Sum(x => x.Quantity);
                }
                else
                {
                    BasketCount = 0;
                }
            }
            else
            {
                BasketCount = 0;
            }
        }
        catch
        {
            BasketCount = 0;
        }

        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
using AppProject.Client.Helpers;
using AppProject.Client.Models.Response.Categories;
using AppProject.Client.Models.Response.Product;
using AppProject.Client.Models.ViewModels.Basket;
using AppProject.Client.Models.ViewModels.Product;
using AppProject.Client.Service.Basket;
using AppProject.Client.Service.Categories;
using AppProject.Client.Service.Products;
using AppProject.Client.Service.Rating;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace AppProject.Client.Pages.Public;

public partial class Home : ComponentBase
{
    [Inject] private IProductsService ProductService { get; set; } = default!;
    [Inject] private ICategoryService CategoryService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IBasketService BasketService { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject] private BasketStateService BasketState { get; set; } = default!;
    [Inject] private IRatingService RatingService { get; set; } = default!;

    private ProductVM productFilter = new ProductVM
    {
        PageNumber = 1,
        pageSize = 3,
        Sort = "PriceDce"
    };

    private int totalPages = 1;
    private bool isLoading = true;

    private List<DataProduct> products = new();
    private IEnumerable<CategoriesRS> categories = new List<CategoriesRS>();

    // قاموس لتخزين تقييمات المنتجات بناءً على الـ ProductId
    private Dictionary<int, double> productRatings = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
        await LoadProductsAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        var result = await CategoryService.GetCategories();
        if (result != null)
        {
            categories = result;
        }
    }

    private async Task LoadProductsAsync()
    {
        isLoading = true;

        var response = await ProductService.GetProducts(productFilter);

        if (response != null && response.Data != null)
        {
            products = response.Data;
            productFilter.TotatlCount = response.TotalCount;
            totalPages = (int)Math.Ceiling((double)response.TotalCount / productFilter.pageSize);

            // جلب تقييمات المنتجات الظاهرة
            await FetchRatingsForProducts();
        }
        else
        {
            products.Clear();
            productFilter.TotatlCount = 0;
            totalPages = 1;
        }

        isLoading = false;
    }

    private async Task FetchRatingsForProducts()
    {
        productRatings.Clear();
        foreach (var prod in products)
        {
            try
            {
                var ratings = await RatingService.GetAllRatingForProductAsync(prod.Id);
                if (ratings != null && ratings.Any())
                {
                    productRatings[prod.Id] = ratings.Average(r => r.Stars);
                }
                else
                {
                    productRatings[prod.Id] = 0;
                }
            }
            catch
            {
                productRatings[prod.Id] = 0;
            }
        }
    }

    private double GetProductRating(int productId)
    {
        return productRatings.TryGetValue(productId, out var rating) ? rating : 0;
    }

    private async Task OnCategorySelected(int? categoryId)
    {
        productFilter.CategoryId = categoryId;
        productFilter.PageNumber = 1;
        await LoadProductsAsync();
    }

    private async Task HandleSearch()
    {
        productFilter.PageNumber = 1;
        await LoadProductsAsync();
    }

    private async Task HandleKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await HandleSearch();
        }
    }

    private async Task HandleReset()
    {
        productFilter.Search = string.Empty;
        productFilter.Sort = "PriceDce";
        productFilter.CategoryId = null;
        productFilter.PageNumber = 1;
        await LoadProductsAsync();
    }

    private async Task OnSortChanged(string sort)
    {
        productFilter.Sort = sort;
        productFilter.PageNumber = 1;
        await LoadProductsAsync();
    }

    private async Task OnPageChanged(int page)
    {
        productFilter.PageNumber = page;
        await LoadProductsAsync();
    }

    private List<string> GetImageUrls(List<Photos>? photos)
    {
        if (photos == null || !photos.Any())
            return new List<string> { $"{AppConstant.BaseApiAddress}Images/default.jpg" };

        return photos
            .Where(p => !string.IsNullOrEmpty(p.ImageName))
            .Select(p => p.ImageName.StartsWith("http")
                ? p.ImageName
                : $"{AppConstant.BaseApiAddress}{p.ImageName}")
            .ToList();
    }

    private async Task AddToCart(DataProduct product)
    {
        try
        {
            string basketId = await LocalStorage.GetItemAsync<string>("basket_id");

            if (string.IsNullOrEmpty(basketId))
            {
                basketId = Guid.NewGuid().ToString();
                await LocalStorage.SetItemAsync("basket_id", basketId);
            }

            var existingBasketRS = await BasketService.GetBasketAsync(basketId);
            CustomerBasketVM currentBasket;

            if (existingBasketRS != null)
            {
                currentBasket = new CustomerBasketVM
                {
                    Id = existingBasketRS.Id ?? basketId,
                    PaymentIntentId = existingBasketRS.PaymentIntentId ?? "string",
                    ClientSecret = existingBasketRS.ClientSecret ?? "string",
                    BasketItems = existingBasketRS.BasketItems?.Select(item => new BasketItems
                    {
                        Id = item.Id,
                        Name = item.Name ?? string.Empty,
                        Description = item.Description ?? string.Empty,
                        Image = item.Image ?? string.Empty,
                        Qunatity = item.Quantity > 0 ? item.Quantity : 1,
                        Price = item.Price,
                        Category = item.Category ?? string.Empty
                    }).ToList() ?? new List<BasketItems>()
                };
            }
            else
            {
                currentBasket = new CustomerBasketVM
                {
                    Id = basketId,
                    PaymentIntentId = "string",
                    ClientSecret = "string",
                    BasketItems = new List<BasketItems>()
                };
            }

            var existingItem = currentBasket.BasketItems.FirstOrDefault(x => x.Id == product.Id);

            if (existingItem != null)
            {
                existingItem.Qunatity += 1;
            }
            else
            {
                currentBasket.BasketItems.Add(new BasketItems
                {
                    Id = product.Id,
                    Name = product.Name ?? string.Empty,
                    Description = product.Description ?? string.Empty,
                    Image = GetImageUrls(product.Photos)?.FirstOrDefault() ?? string.Empty,
                    Price = product.NewPrice,
                    Qunatity = 1,
                    Category = product.CategoryName ?? string.Empty
                });
            }

            var result = await BasketService.AddOrUpdateBasketAsync(currentBasket);

            if (result != null && result.StatusCode != 400)
            {
                await BasketState.UpdateBasketCountAsync();
                Snackbar.Add("تمت إضافة المنتج إلى السلة بنجاح", Severity.Success);
            }
            else
            {
                Snackbar.Add($"حدث خطأ أثناء الإضافة: {result?.Message ?? "Bad Request"}", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddToCart: {ex.Message}");
            Snackbar.Add("حدث خطأ غير متوقع", Severity.Error);
        }
    }

    private void ViewDetails(DataProduct item)
    {
        if (item != null)
        {
            NavigationManager.NavigateTo($"/ProductDetails/{item.Id}");
        }
    }

    private void ToggleBookmark(DataProduct item, bool status)
    {
    }
}
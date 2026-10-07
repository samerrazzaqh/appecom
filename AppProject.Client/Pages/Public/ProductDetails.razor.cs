using AppProject.Client.Helpers;
using AppProject.Client.Models.Response.Product;
using AppProject.Client.Models.Response.Rating;
using AppProject.Client.Models.ViewModels.Basket;
using AppProject.Client.Models.ViewModels.Product;
using AppProject.Client.Models.ViewModels.Rating;
using AppProject.Client.Service.Basket;
using AppProject.Client.Service.Products;
using AppProject.Client.Service.Rating;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace AppProject.Client.Pages.Public;

public partial class ProductDetails : ComponentBase
{
    [Parameter] public int ProductId { get; set; }
    [SupplyParameterFromQuery] public int? Quantity { get; set; }

    [Inject] private IProductsService ProductService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IBasketService BasketService { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject] private BasketStateService BasketState { get; set; } = default!;
    [Inject] private IRatingService RatingService { get; set; } = default!;

    private GetProductRS? product;
    private List<GetProductRS> similarProducts = new();
    private List<string> productImages = new();
    private string selectedImage = string.Empty;

    private bool isLoading = true;
    private bool isWishlist = false;
    private int quantity = 1;

    // متغيرات التقييمات
    private List<RatingRS> productReviews = new();
    private RatingVM newRating = new() { Stars = 5 };
    private bool isLoadingRatings = true;
    private bool isSubmittingRating = false;
    private double averageRating = 0;

    // متغيرات التكبير Zoom
    private bool isZoomed = false;
    private double zoomX = 50;
    private double zoomY = 50;
    private const double ContainerWidth = 380;

    private string ZoomStyle => isZoomed
        ? $"transform-origin: {zoomX}% {zoomY}%;"
        : string.Empty;

    protected override async Task OnParametersSetAsync()
    {
        if (Quantity.HasValue && Quantity.Value > 0)
        {
            quantity = Quantity.Value;
        }

        await LoadProductData();
        await LoadRatings();
    }

    private async Task LoadProductData()
    {
        isLoading = true;
        similarProducts.Clear();

        if (ProductId > 0)
        {
            product = await ProductService.GetByIdAsync(ProductId);
        }

        if (product != null)
        {
            productImages = GetImageUrls(product.Photos);
            selectedImage = productImages.FirstOrDefault() ?? $"{AppConstant.BaseApiAddress}Images/default.jpg";

            try
            {
                var similarResponse = await ProductService.GetProducts(new ProductVM
                {
                    PageNumber = 1,
                    pageSize = 6
                });

                if (similarResponse?.Data != null && similarResponse.Data.Any())
                {
                    var filtered = similarResponse.Data
                        .Where(p => p.Id != product.Id &&
                                    string.Equals(p.CategoryName, product.CategoryName,
                                        StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (!filtered.Any())
                    {
                        filtered = similarResponse.Data
                            .Where(p => p.Id != product.Id)
                            .ToList();
                    }

                    similarProducts = filtered
                        .Take(4)
                        .Select(p => new GetProductRS
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            NewPrice = (decimal)p.NewPrice,
                            OldPrice = (decimal)p.OldPrice,
                            CategoryName = p.CategoryName,
                            Photos = p.Photos?.Select(photo => new PhotoVM
                            {
                                ImageName = photo.ImageName,
                                ProductId = p.Id
                            }).ToList() ?? new()
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading similar products: {ex.Message}");
            }
        }

        isLoading = false;
        StateHasChanged();
    }

    private async Task LoadRatings()
    {
        isLoadingRatings = true;
        try
        {
            var reviews = await RatingService.GetAllRatingForProductAsync(ProductId);
            productReviews = reviews?.ToList() ?? new List<RatingRS>();

            if (productReviews.Any())
            {
                averageRating = productReviews.Average(r => r.Stars);
            }
            else
            {
                averageRating = 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading ratings: {ex.Message}");
        }
        finally
        {
            isLoadingRatings = false;
            StateHasChanged();
        }
    }

    private async Task SubmitRating()
    {
        if (newRating.Stars < 1 || newRating.Stars > 5)
        {
            Snackbar.Add("Please select a star rating.", Severity.Warning);
            return;
        }

        isSubmittingRating = true;
        try
        {
            newRating.ProductId = ProductId;
            var isSuccess = await RatingService.AddRatingAsync(newRating);

            if (isSuccess)
            {
                Snackbar.Add("The evaluation has been sent successfully!", Severity.Success);
                newRating = new RatingVM { Stars = 5, Content = string.Empty };
                await LoadRatings();
            }
            else
            {
                Snackbar.Add("Failed to submit the rating. You may have already rated this product, or you need to log in.", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error submitting rating: {ex.Message}");
            Snackbar.Add("An error occurred while sending the rating.", Severity.Error);
        }
        finally
        {
            isSubmittingRating = false;
            StateHasChanged();
        }
    }

    private void HandleMouseMove(MouseEventArgs e)
    {
        isZoomed = true;
        zoomX = Math.Clamp((e.OffsetX / ContainerWidth) * 100, 0, 100);
        zoomY = Math.Clamp((e.OffsetY / ContainerWidth) * 100, 0, 100);
    }

    private void HandleMouseLeave()
    {
        isZoomed = false;
    }

    private void SelectImage(string img)
    {
        selectedImage = img;
    }

    private void IncreaseQuantity()
    {
        quantity++;
    }

    private void DecreaseQuantity()
    {
        if (quantity > 1) quantity--;
    }

    private void ToggleWishlist()
    {
        isWishlist = !isWishlist;
    }

    private async Task AddToCart()
    {
        if (product == null) return;

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

            if (quantity <= 0)
            {
                if (existingItem != null)
                {
                    currentBasket.BasketItems.Remove(existingItem);
                }
            }
            else if (existingItem != null)
            {
                existingItem.Qunatity = quantity;
            }
            else
            {
                currentBasket.BasketItems.Add(new BasketItems
                {
                    Id = product.Id,
                    Name = product.Name ?? string.Empty,
                    Description = product.Description ?? string.Empty,
                    Image = productImages.FirstOrDefault() ?? string.Empty,
                    Price = product.NewPrice,
                    Qunatity = quantity,
                    Category = product.CategoryName ?? string.Empty
                });
            }

            if (!currentBasket.BasketItems.Any())
            {
                await BasketService.DeleteBasketAsync(basketId);
                await LocalStorage.RemoveItemAsync("basket_id");
                await BasketState.UpdateBasketCountAsync();
                Snackbar.Add("The product has been removed and the cart emptied.", Severity.Info);
                return;
            }

            var result = await BasketService.AddOrUpdateBasketAsync(currentBasket);

            if (result != null && result.StatusCode != 400)
            {
                await BasketState.UpdateBasketCountAsync();
                Snackbar.Add("The cart has been successfully updated.", Severity.Success);
            }
            else
            {
                Snackbar.Add($"An error occurred during the update: {result?.Message ?? "Bad Request"}", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddToCart: {ex.Message}");
            Snackbar.Add("An unexpected error occurred.", Severity.Error);
        }
    }

    private void NavigateToShopping()
    {
        NavigationManager.NavigateTo("/shopping");
    }

    private void NavigateToProduct(int id)
    {
        NavigationManager.NavigateTo($"/ProductDetails/{id}");
    }

    private int GetDiscountPercentage(decimal oldPrice, decimal newPrice)
    {
        if (oldPrice <= 0 || oldPrice <= newPrice) return 0;
        return (int)Math.Round(((oldPrice - newPrice) / oldPrice) * 100);
    }

    private List<string> GetImageUrls(List<PhotoVM>? photos)
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
}
using Microsoft.AspNetCore.Components;

namespace AppProject.Client.Pages.Component;

public partial class ProductCard : ComponentBase
{
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Description { get; set; } = "Lorem ipsum dolor sit amet consectetur, adipisicing elit.";
    [Parameter] public decimal Price { get; set; } = 900.00m;
    [Parameter] public decimal? OldPrice { get; set; } = 1000.00m;
    [Parameter] public List<string> Images { get; set; } = new();
    [Parameter] public bool IsBookmarked { get; set; }

    // البرامتر الجديد المضاف لعرض التقييمات
    [Parameter] public double Rating { get; set; } = 0;

    [Parameter] public EventCallback OnAddToCart { get; set; }
    [Parameter] public EventCallback OnDetails { get; set; }
    [Parameter] public EventCallback<bool> OnBookmarkToggle { get; set; }

    private string SelectedImage { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        if (Images != null && Images.Any())
        {
            SelectedImage = Images.First();
        }
    }

    protected override void OnParametersSet()
    {
        if (Images != null && Images.Any() && string.IsNullOrEmpty(SelectedImage))
        {
            SelectedImage = Images.First();
        }
    }

    private void SelectImage(string imgUrl)
    {
        SelectedImage = imgUrl;
    }

    private async Task OnBookmarkClick()
    {
        IsBookmarked = !IsBookmarked;
        await OnBookmarkToggle.InvokeAsync(IsBookmarked);
    }
}
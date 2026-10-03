using AppProject.Client.Models.ViewModels.Rating;
using AppProject.Client.Service.Rating;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Component;

public partial class RatingDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public int ProductId { get; set; }
    [Parameter] public string ProductName { get; set; } = string.Empty;

    [Inject] private IRatingService? RatingService { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private RatingVM newRating = new() { Stars = 5 };
    private bool isSubmitting;

    protected override void OnInitialized()
    {
        newRating.ProductId = ProductId;
    }

    private void Cancel() => MudDialog.Cancel();
    private async Task Submit()
    {
        if (newRating.Stars < 1 || newRating.Stars > 5)
        {
            Snackbar.Add("يرجى اختيار التقييم بالنجوم.", Severity.Warning);
            return;
        }

        isSubmitting = true;
        try
        {
            newRating.ProductId = ProductId;
            var isSuccess = await RatingService.AddRatingAsync(newRating);

            if (isSuccess)
            {
                Snackbar.Add("تم إضافة تقييمك بنجاح!", Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add("فشل إرسال التقييم. ربما قمت بتقييم هذا المنتج مسبقاً أو يلزمك تسجيل الدخول.", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error submitting rating: {ex.Message}");
            Snackbar.Add("حدث خطأ أثناء إرسال التقييم.", Severity.Error);
        }
        finally
        {
            isSubmitting = false;
            StateHasChanged();
        }
    }

}
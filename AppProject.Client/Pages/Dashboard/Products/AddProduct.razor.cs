using AppProject.Client.Models.Response.Categories;
using AppProject.Client.Models.ViewModels.Product;
using AppProject.Client.Service.Categories;
using AppProject.Client.Service.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Products;

public partial class AddProduct : ComponentBase
{
    [Inject]
    public ICategoryService? CategoryService { get; set; }
    [Inject]
    public IProductsService ProductService { get; set; }
    
    [Inject]
    public ISnackbar Snackbar { get; set; }
    
    [Inject]
    public NavigationManager? NavigationManager { get; set; }
    
    private List<CategoriesRS> _categories = new();
    private AddProductVM _productModel = new();
    private List<FilePreviewModel> _filePreviews = new();
    private bool _isSubmitting = false;

    private class FilePreviewModel
    {
        public IBrowserFile File { get; set; } = null!;
        public string Base64Url { get; set; } = string.Empty;
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (CategoryService != null)
            {
                var result = await CategoryService.GetCategories();
                if (result != null)
                {
                    _categories = result.ToList();
                    if (_categories.Any())
                    {
                        _productModel.CategoryId = _categories.First().id;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading categories: {ex.Message}", Severity.Error);
        }
    }

    private async Task OnInputFileChanged(InputFileChangeEventArgs e)
    {
        _productModel.Photo ??= new List<IBrowserFile>();
        var selectedFiles = e.GetMultipleFiles();

        foreach (var file in selectedFiles)
        {
            if (!_productModel.Photo.Any(f => f.Name == file.Name))
            {
                _productModel.Photo.Add(file);
                var resizedImage = await file.RequestImageFileAsync(file.ContentType, 300, 300);
                using var stream = resizedImage.OpenReadStream(maxAllowedSize: 1024 * 1024 * 15);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                
                var base64 = $"data:{file.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";

                _filePreviews.Add(new FilePreviewModel
                {
                    File = file,
                    Base64Url = base64
                });
            }
        }
    }

    private void RemoveFile(FilePreviewModel preview)
    {
        _productModel.Photo?.Remove(preview.File);
        _filePreviews.Remove(preview);
    }

    private async Task HandleSubmit()
    {
        _isSubmitting = true;

        try
        {
            var result = await ProductService.AddAsync(_productModel);

            if (result)
            {
                Snackbar.Add("The product has been successfully added!", Severity.Success);
                NavigationManager!.NavigateTo("javascript:history.back()");
            }
            else
            {
                Snackbar.Add("An error occurred while adding the product. Check the entered data.", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"error: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isSubmitting = false;
        }
    }
}
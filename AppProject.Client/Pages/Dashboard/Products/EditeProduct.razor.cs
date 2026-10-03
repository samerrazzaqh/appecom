using AppProject.Client.Helpers;
using AppProject.Client.Models.Response.Categories;
using AppProject.Client.Models.Response.Product;
using AppProject.Client.Models.ViewModels.Product;
using AppProject.Client.Service.Categories;
using AppProject.Client.Service.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Products;

public partial class EditeProduct : ComponentBase
{
    [SupplyParameterFromQuery(Name = "ProductID")]
    public int Id { get; set; }

    [Inject] HttpClient HttpClient { get; set; }

    [Inject] public NavigationManager? NavigationManager { get; set; }

    [Inject] public ISnackbar Snackbar { get; set; }

    [Inject] public IProductsService? ProductService { get; set; }

    [Inject] public ICategoryService? CategoryService { get; set; }

    private List<CategoriesRS> _categoriesRS = new();
    private GetProductRS getProductRs = new();


    private AddProductVM _productModel = new();
    private List<FilePreviewModel> _filePreviews = new();

    private bool _isLoading = true;
    private bool _isSubmitting = false;

    private class FilePreviewModel
    {
        public IBrowserFile File { get; set; } = null!;
        public string Base64Url { get; set; } = string.Empty;
    }

    private class LoadedBrowserFile : IBrowserFile
    {
        private readonly byte[] _fileBytes;

        public LoadedBrowserFile(string name, string contentType, byte[] fileBytes)
        {
            Name = name;
            ContentType = contentType;
            _fileBytes = fileBytes;
            Size = fileBytes.Length;
            LastModified = DateTimeOffset.Now;
        }

        public string Name { get; }
        public DateTimeOffset LastModified { get; }
        public long Size { get; }
        public string ContentType { get; }

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            return new MemoryStream(_fileBytes);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
        await LoadProductDataAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var result = await CategoryService.GetCategories();
            if (result != null)
            {
                _categoriesRS = result.ToList();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error fetching categories: {ex.Message}", Severity.Error);
        }
    }

    private async Task LoadProductDataAsync()
    {
        _isLoading = true;
        try
        {
            var productData = await ProductService.GetByIdAsync(Id);

            if (productData != null)
            {
                _productModel.Name = productData.Name;
                _productModel.Description = productData.Description;
                _productModel.NewPrice = productData.NewPrice;
                _productModel.OldPrice = productData.OldPrice;
                _productModel.Photo = new List<IBrowserFile>();

                var matchedCategory = _categoriesRS.FirstOrDefault(c => c.name == productData.CategoryName);
                if (matchedCategory != null)
                {
                    _productModel.CategoryId = matchedCategory.id;
                }

                if (productData.Photos != null && productData.Photos.Any())
                {
                    foreach (var photo in productData.Photos)
                    {
                        try
                        {
                            // 1. تقسيم المسار لتشفير الأجزاء التي تحتوي على مسافات فقط دون تشفير الـ Slashes /
                            var pathSegments = photo.ImageName.Split('/', StringSplitOptions.RemoveEmptyEntries);
                            var encodedPath = string.Join('/', pathSegments.Select(segment => Uri.EscapeDataString(segment)));
            
                            var imageUrl = $"{AppConstant.BaseApiAddress}{encodedPath}";

                            // 2. طلب الصورة بعد معالجة الرابط
                            var imageBytes = await HttpClient.GetByteArrayAsync(imageUrl);

                            string fileName = Path.GetFileName(photo.ImageName);
                            string extension = Path.GetExtension(fileName).ToLower();
                            string contentType = extension switch
                            {
                                ".jpg" or ".jpeg" => "image/jpeg",
                                ".png" => "image/png",
                                ".gif" => "image/gif",
                                ".webp" => "image/webp",
                                _ => "application/octet-stream"
                            };

                            IBrowserFile browserFile = new LoadedBrowserFile(fileName, contentType, imageBytes);

                            _productModel.Photo.Add(browserFile);
                            _filePreviews.Add(new FilePreviewModel
                            {
                                File = browserFile,
                                Base64Url = $"data:{contentType};base64,{Convert.ToBase64String(imageBytes)}"
                            });
                        }
                        catch (Exception ex)
                        {
                            // طباعة سبب الفشل في الـ Console لمعرفة الصورة التي فشلت
                            Console.WriteLine($"Error loading image '{photo.ImageName}': {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                Snackbar.Add("The requested product was not found.", Severity.Warning);
                NavigationManager!.NavigateTo("javascript:history.back()");
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading product data: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isLoading = false;
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
            var updateModel = new UpdateProductVM
            {
                Id = Id,
                Name = _productModel.Name,
                Description = _productModel.Description,
                NewPrice = _productModel.NewPrice,
                OldPrice = _productModel.OldPrice,
                CategoryId = _productModel.CategoryId,
                Photo = _productModel.Photo
            };

            var result = await ProductService.UpdateAsync(updateModel);

            if (result)
            {
                Snackbar.Add("The product has been successfully updated!", Severity.Success);
                NavigationManager!.NavigateTo("javascript:history.back()");
            }
            else
            {
                Snackbar.Add("An error occurred while editing the product. Check the console for details.", Severity.Error);
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
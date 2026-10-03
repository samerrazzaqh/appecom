using AppProject.Client.Authentication;
using AppProject.Client.Models.Response.Categories;
using AppProject.Client.Models.Response.Product;
using AppProject.Client.Models.ViewModels.Product;
using AppProject.Client.Service.Categories;
using AppProject.Client.Service.Products;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Products;

public partial class ListProducts : ComponentBase
{
    [Inject] public NavigationManager? NavigationManager { get; set; }
    [Inject] public IDialogService? DialogService { get; set; }
    [Inject] public IProductsService? ProductService { get; set; }
    [Inject] public ICategoryService? CategoryService { get; set; }
    [Inject] public CheckAndRefreshToken CheckAndRefreshToken { get; set; }

    private MudTable<DataProduct> _table = null!;
    private string _searchString = string.Empty;

    // متغيرة الحالة للقيم الديناميكية
    public int? SelectedCategoryId { get; set; }
    public string SelectedSort { get; set; } = "PriceDce";

    // نموذج لبيانات الفئات (يمكنك استبداله بالـ DTO الخاص بك)
    public IEnumerable<CategoriesRS> CategoriesRS = new List<CategoriesRS>();

    protected override async Task OnInitializedAsync()
    {
        await CheckAndRefreshToken.SetAuthorizeHeader();
        await LoadCategories();
    }

    private async Task LoadCategories()
    {
        // استدعاء الفئات من الـ Service الخاصة بك (تعديل حسب الـ Service المتاحة لديك)
        CategoriesRS = await CategoryService.GetCategories();
    }

    private async Task<TableData<DataProduct>> LoadProducts(TableState state, CancellationToken token)
    {
        var paramsVm = new ProductVM
        {
            PageNumber = state.Page + 1, 
            pageSize = state.PageSize,
            Search = _searchString,
            Sort = SelectedSort,                 // ديناميكي
            CategoryId = SelectedCategoryId      // ديناميكي
        };

        var result = await ProductService!.GetProducts(paramsVm);

        if (result != null)
        {
            return new TableData<DataProduct>
            {
                TotalItems = result.TotalCount,
                Items = result.Data
            };
        }

        return new TableData<DataProduct> { TotalItems = 0, Items = new List<DataProduct>() };
    }
    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData(); // يعيد جلب البيانات ديناميكياً من السيرفر فور كتابة أي حرف
    }

    private void OnCategoryChanged(int? categoryId)
    {
        SelectedCategoryId = categoryId;
        _table.ReloadServerData();
    }

    private void OnSortChanged(string sort)
    {
        SelectedSort = sort;
        _table.ReloadServerData();
    }

    public async Task EditeProduct(int Id)
    {
        NavigationManager?.NavigateTo($"/EditeProduct?ProductID={Id}");
    }

    public async Task OpenDialogDelete(int Id, string name)
    {
        var parameters = new DialogParameters { ["productId"] = Id };
        var dialog = await DialogService!.ShowAsync<DeleteProduct>($"Are you Delete {name}", parameters);
        var result = await dialog.Result;
        
        if (result != null && !result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }
}
using AppProject.Client.Authentication;
using AppProject.Client.Models.Response.Categories;
using AppProject.Client.Service.Categories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Categories;

public partial class ListCategory : ComponentBase
{
    [Inject] public ICategoryService? CategoryService { get; set; }
    public IEnumerable<CategoriesRS> CategoriesRS = new List<CategoriesRS>();

    [Inject] public NavigationManager? NavigationManager { get; set; }
    [Inject] public CheckAndRefreshToken CheckAndRefreshToken { get; set; }


    private bool isVisible;

    protected override async Task OnInitializedAsync()
    {
        isVisible = true;
        await CheckAndRefreshToken.SetAuthorizeHeader();
        CategoriesRS = (await CategoryService!.GetCategories())!.ToList();
        StateHasChanged();
        isVisible = false;
    }

    public bool _sortNameByLength;
    public SortMode _sortMode = SortMode.Multiple;

    //// custom sort by name length
    private Func<CategoriesRS, object> _sortBy => x =>
    {
        if (_sortNameByLength)
            return x.id;
        else
            return x.name!;
    };


    public string? _searchString;

    public Func<CategoriesRS, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (x.name!.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };


    [Inject] public IDialogService? DialogService { get; set; }
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

    public async Task OpenDialogCreate(DialogOptions options)
    {
        var dialog = await DialogService!.ShowAsync<DialogCreate>("Add New Category", options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            CategoriesRS = (await CategoryService!.GetCategories())!.ToList();
        }
    }

    public async Task OpenDialogEdite(DialogOptions options, CategoriesRS model)
    {
        var parameters = new DialogParameters();
        parameters.Add("category", model);
        var dialog = await DialogService!.ShowAsync<DialogEdite>($"Edite", parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            CategoriesRS = (await CategoryService!.GetCategories())!.ToList();
        }
    }


    public async Task OpenDialogDelete(CategoriesRS model)
    {
        var parameters = new DialogParameters();
        parameters.Add("categoryId", model.id);
        var dialog = await DialogService!.ShowAsync<DialogDelete>($"Are you Delete {model.name}", parameters);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            CategoriesRS = (await CategoryService!.GetCategories())!.ToList();
        }
    }
}
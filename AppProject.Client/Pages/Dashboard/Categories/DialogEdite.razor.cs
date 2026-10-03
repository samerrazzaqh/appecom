using AppProject.Client.Models.Response.Categories;
using AppProject.Client.Models.ViewModels.Categories;
using AppProject.Client.Service.Categories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Categories;

public partial class DialogEdite : ComponentBase
{
    public CategoriesRS? CategoriesRs { get; set; } = new CategoriesRS();

    public CategoryVM? EditeCategoryVM { get; set; } = new CategoryVM();

    [Inject] public ICategoryService? CategoryService { get; set; }


    public bool ShowMessage;
    public bool Successful;
    public string? Message;

    [Parameter] public CategoriesRS? category { get; set; }


    protected async override Task OnInitializedAsync()
    {
        CategoriesRs = await CategoryService!.GetCategory(category!.id);
        EditeCategoryVM.id = CategoriesRs.id;
        EditeCategoryVM.name = CategoriesRs.name;
        EditeCategoryVM.description = CategoriesRs.description;
    }


    protected async Task HandleEdite()
    {
        ShowMessage = false;
        var result = await CategoryService!.UpdateCategory(EditeCategoryVM!);

        if (result!.StatusCode == 200)
        {
            Submit();
        }
        else
        {
            ShowMessage = true;
            Message = result.Message;
        }
    }

    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }

    public void Submit()
    {
        MudDialog!.Close(DialogResult.Ok(true));
    }

    public void Cancel() => MudDialog!.Cancel();
}
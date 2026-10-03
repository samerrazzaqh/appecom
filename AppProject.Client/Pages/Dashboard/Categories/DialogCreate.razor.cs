using AppProject.Client.Models.ViewModels.Categories;
using AppProject.Client.Service.Categories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Categories;

public partial class DialogCreate : ComponentBase
{
    [Inject] public ICategoryService? categoryService { get; set; }
    public CategoryVM categoryVm = new CategoryVM();
    public bool ShowMessage;
    public string? Message;


    [Inject] public NavigationManager? NavigationManager { get; set; }


    public async Task HandleCreate()
    {
        ShowMessage = false;
        var result = await categoryService!.CreateCategory(categoryVm);
        if (result.StatusCode == 200)
        {
            Submit();
        }
        else
        {
            Message = result.Message;
            ShowMessage = true;
        }
        StateHasChanged();
    }


    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }

    public void Submit()
    {
        MudDialog!.Close(DialogResult.Ok(true));
    }

    public void Cancel() => MudDialog!.Cancel();
}
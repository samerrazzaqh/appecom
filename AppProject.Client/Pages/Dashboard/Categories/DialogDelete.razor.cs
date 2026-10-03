using AppProject.Client.Service.Categories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Categories;

public partial class DialogDelete : ComponentBase
{
    [Inject] public ICategoryService? categoryService { get; set; }


    [Parameter] public int categoryId { get; set; }

    public async Task HandleDelete()
    {
        await categoryService!.DeleteCategory(categoryId);
    }


    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }

    public async Task Submit()
    {
        await HandleDelete();
        MudDialog!.Close(DialogResult.Ok(true));
        StateHasChanged();
    }

    public void Cancel() => MudDialog!.Cancel();
}
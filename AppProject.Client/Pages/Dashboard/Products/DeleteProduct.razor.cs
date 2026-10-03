using AppProject.Client.Service.Products;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppProject.Client.Pages.Dashboard.Products;

public partial class DeleteProduct
{
    [Inject]
    public IProductsService? ProductService { get; set; }

    [Parameter]
    public int productId { get; set; }

    public async Task HandleDelete()
    {
        await ProductService!.DeleteAsync(productId);
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
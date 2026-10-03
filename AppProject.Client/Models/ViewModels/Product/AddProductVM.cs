using Microsoft.AspNetCore.Components.Forms;

namespace AppProject.Client.Models.ViewModels.Product;

public record AddProductVM
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? NewPrice { get; set; }
    public decimal? OldPrice { get; set; }
    public int? CategoryId { get; set; }
    public List<IBrowserFile>? Photo { get; set; }
}

public record UpdateProductVM : AddProductVM
{
    public int Id { get; set; }
    // قائمة المسارات القديمة للصور التي أبقاها المستخدم ولم يقم بحذفها
    public List<string> ExistingPhotoPaths { get; set; } = new();
}
using System.ComponentModel.DataAnnotations;

namespace AppProject.Client.Models.ViewModels.Categories;

public class CategoryVM
{
    public int id { get; set; }

    [StringLength(50), Display(Name = "Category Name"), Required(ErrorMessage = "Category is required")]
    public string? name { get; set; }
    
    public string? description { get; set; }
}
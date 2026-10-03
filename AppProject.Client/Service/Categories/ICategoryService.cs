using AppProject.Client.Models.Response.Categories;
using AppProject.Client.Models.ViewModels.Categories;

namespace AppProject.Client.Service.Categories;

public interface ICategoryService
{
    Task<IEnumerable<CategoriesRS>?> GetCategories();
    Task<CategoriesRS> CreateCategory(CategoryVM newCategory);
    Task<CategoriesRS?> GetCategory(int id);
    Task<CategoriesRS?> UpdateCategory(CategoryVM updatedCategory);
    Task DeleteCategory(int id);
}
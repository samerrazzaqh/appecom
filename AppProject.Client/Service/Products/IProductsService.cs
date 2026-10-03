using AppProject.Client.Models.Response.Product;
using AppProject.Client.Models.ViewModels.Product;

namespace AppProject.Client.Service.Products;

public interface IProductsService
{
    Task<ProductRS<DataProduct>?> GetProducts(ProductVM productVm);
    
    Task<bool> AddAsync(AddProductVM addProductVm);
    Task<GetProductRS> GetByIdAsync(int Id);
    
    Task<bool> UpdateAsync(UpdateProductVM updateProductVM);
    Task DeleteAsync(int productId);
}
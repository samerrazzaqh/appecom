using Core.Entities.Product;
using Core.interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositries
{
    public class CategoryRepositry : GenericRepositry<Category>, ICategoryRepositry
    {
        public CategoryRepositry(AppDbContext context) : base(context)
        {
        }
    }
}

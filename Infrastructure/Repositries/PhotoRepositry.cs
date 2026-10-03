using Core.Entities.Product;
using Core.interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositries
{
    public class PhotoRepositry : GenericRepositry<Photo>, IPhotoRepositry
    {
        public PhotoRepositry(AppDbContext context) : base(context)
        {
        }
    }
}

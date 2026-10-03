using Core.DTO;
using Core.Entities;
using Core.Entities.Product;
using Core.interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositries
{
    public class RatingRepositry : IRating
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public RatingRepositry(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // public async Task<bool> AddRatingAsync(RatingDTO ratingDTO, string email)
        // {
        //     var finduser = await _userManager.FindByEmailAsync(email);
        //
        //     if (await _context.Ratings.AsNoTracking().AnyAsync(m => m.AppUserId == finduser.Id && m.ProductId == ratingDTO.productId))
        //     {
        //         return false;
        //     }
        //
        //     var rating = new Rating()
        //     {
        //         AppUserId = finduser.Id,
        //         ProductId = ratingDTO.productId,
        //         Stars = ratingDTO.stars,
        //         content = ratingDTO.content,
        //
        //     };
        //     await _context.Ratings.AddAsync(rating);
        //     await _context.SaveChangesAsync();
        //
        //     var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == ratingDTO.productId);
        //
        //     var ratings = await _context.Ratings.AsNoTracking().Where(m => m.ProductId == product.Id).ToListAsync();
        //
        //     if (ratings.Count > 0)
        //     {
        //         double average = ratings.Average(m => m.Stars);
        //         double roundedReview = Math.Round(average * 2, mode: MidpointRounding.AwayFromZero) / 2;
        //         product.rating = roundedReview;
        //     }
        //     else
        //     {
        //         product.rating = ratingDTO.stars;
        //     }
        //     await _context.SaveChangesAsync();
        //     return true;
        //
        // }

        
        public async Task<bool> AddRatingAsync(RatingDTO ratingDTO, string email)
        {
            // 1. التحقق من وجود المستخدم لتجنب NullReferenceException
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            // 2. التحقق من وجود المنتج المقيّم
            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == ratingDTO.productId);
            if (product == null)
            {
                return false;
            }

            // 3. التحقق مما إذا كان المستخدم قد قيّم المنتج سابقاً
            bool hasRated = await _context.Ratings.AsNoTracking().AnyAsync(m => m.AppUserId == user.Id && m.ProductId == ratingDTO.productId);
            if (hasRated)
            {
                return false;
            }

            // 4. إنشاء التقييم الجديد
            var rating = new Rating
            {
                AppUserId = user.Id,
                ProductId = ratingDTO.productId,
                Stars = ratingDTO.stars,
                content = ratingDTO.content
            };

            await _context.Ratings.AddAsync(rating);

            // 5. حساب المتوسط مباشرة في قاعدة البيانات بدلاً من تحميل كل السجلات للذاكرة
            // نستخدم (double?) لتجنب الاستثناءات في حال عدم وجود تقييمات سابقاً
            var averageStars = await _context.Ratings
                .Where(m => m.ProductId == ratingDTO.productId)
                .AverageAsync(m => (double?)m.Stars) ?? ratingDTO.stars;

            // 6. تقريب النتيجة لأقرب نصف نجمة (0.5)
            double roundedRating = Math.Round(averageStars * 2, MidpointRounding.AwayFromZero) / 2;
            product.rating = roundedRating;

            // 7. حفظ التقييم وتحديث المنتج في طلب واحد لقاعدة البيانات (Single SaveChanges)
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<IReadOnlyList<ReturnRatingDTO>> GetAllRatingForProductAsync(int productId)
        {
            var ratings = await _context.Ratings.Include(m => m.AppUser)
                 .AsNoTracking().Where(m => m.ProductId == productId).ToListAsync();

            return ratings.Select(m => new ReturnRatingDTO
            {
                content = m.content,
                ReviewTime = m.Review,
                stars = m.Stars,
                userName = m.AppUser.UserName,
            }).ToList();
        }
    }
}

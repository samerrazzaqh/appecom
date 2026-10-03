using AppProject.Client.Models.Response.Rating;
using AppProject.Client.Models.ViewModels.Rating;

namespace AppProject.Client.Service.Rating;

public interface IRatingService
{
    Task<bool> AddRatingAsync(RatingVM ratingVm);
    Task<IEnumerable<RatingRS>> GetAllRatingForProductAsync(int productId);
}
using ECommerce.Core.DTOs.Review;
using ECommerce.Core.Entities;
using ECommerce.Core.Result_Pattern;

namespace ECommerce.Core.Services
{
    public interface IReviewService
    {
        public Task<Result<ICollection<ReviewListDto>>> GetAll(int productId);
        public Task<Result<Review>> AddReview(ReviewDto reviewDto);
        public Task<Result> Delete(int id);
    }
}

using AutoMapper;
using ECommerce.Core.DTOs.Review;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;

namespace ECommerce.Infrastructure.Services
{
    public class ReviewService:IReviewService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ReviewService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }


        public async Task<Result<ICollection<ReviewListDto>>> GetAll(int productId)
        {
            var result = await unitOfWork.Reviews.FindAllAsync((x => x.ProductId == productId));
            if (!result.IsSuccess)
            {
                return Result<ICollection<ReviewListDto>>.Failure("An error occurred while retrieving Reviews.",500);
            }
            var ListReviews = mapper.Map<ICollection<ReviewListDto>>(result.Value);
            return Result<ICollection<ReviewListDto>>.Success(ListReviews);
        }

        
        public async Task<Result<Review>> AddReview(ReviewDto reviewDto)
        {
            Review review = mapper.Map<Review>(reviewDto);
            var result = await unitOfWork.Reviews.AddAsync(review);
            if (!result.IsSuccess)
            {
                return Result<Review>.Failure(result.Error);
            }
            await unitOfWork.Completed();
            return Result<Review>.Success(result.Value);
        }


        public async Task<Result> Delete(int id)
        {
            var result = await unitOfWork.Reviews.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                return Result.Failure($"Reviews with ID {id} not found.",404);
            }
            var DeleteResult = unitOfWork.Reviews.Delete(result.Value);
            if (!DeleteResult.IsSuccess)
            {
                return Result.Failure(DeleteResult.Error);
            }
            await unitOfWork.Completed();
            return Result.Success();
        }


    }
}

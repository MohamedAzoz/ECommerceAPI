using AutoMapper;
using ECommerce.Core.DTOs.Review;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        //GET    /api/products/{id}/reviews
        //POST   /api/products/{id}/reviews
        //PUT    /api/reviews/{id}
        //DELETE /api/reviews/{id}

        private readonly IReviewService reviewService;

        public ReviewsController(IReviewService _reviewService)
        {
            reviewService = _reviewService;
        }

        /// <summary>
        /// Retrieves all reviews for a specified product.
        /// </summary>
        /// <param name="productId">The unique identifier of the product for which to retrieve reviews. Must be a positive integer.</param>
        /// <returns>An <see cref="ActionResult"/> containing a collection of <see cref="ReviewListDto"/> objects representing
        /// the reviews for the specified product. Returns a 500 status code if an error occurs during retrieval.</returns>
        [HttpGet("{productId:int}")]
        public async Task<ActionResult> GetAll(int productId)
        {
            var result =await reviewService.GetAll(productId);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Adds a new review to the system.
        /// </summary>
        /// <remarks>This method maps the provided <paramref name="reviewDto"/> to a <see cref="Review"/>
        /// entity and attempts to add it to the data store. It ensures that the model state is valid before proceeding
        /// with the addition.</remarks>
        /// <param name="reviewDto">The data transfer object containing the review details to be added.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation. Returns <see cref="BadRequest"/> if
        /// the model state is invalid or if the review could not be added successfully. Returns <see cref="Ok"/> with
        /// the added review details if the operation is successful.</returns>
        [HttpPost]
        public async Task<ActionResult> AddReview(ReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result =await reviewService.AddReview(reviewDto);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);
        }
       
        /// <summary>
        /// Deletes a review with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the review to delete. Must be a positive integer.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the delete operation. Returns <see cref="NotFound"/>
        /// if the review with the specified ID does not exist, <see cref="BadRequest"/> if the deletion fails, or <see
        /// cref="NoContent"/> if the deletion is successful.</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result =await reviewService.Delete(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return NoContent();
        }


    }
}

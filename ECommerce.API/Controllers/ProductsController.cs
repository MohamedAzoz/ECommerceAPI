using AutoMapper;
using ECommerce.Core.DTOs.Product;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductService productService;

        public ProductsController(IProductService _productService)
        {
            productService = _productService;
        }

        /// <summary>
        /// Retrieves all products asynchronously.
        /// </summary>
        /// <remarks>This method uses the unit of work pattern to access the product repository and 
        /// returns the result as an HTTP response.</remarks>
        /// <returns>An <see cref="ActionResult"/> containing a list of all products if successful;  otherwise, a status code 500
        /// with an error message if an error occurs during retrieval.</returns>
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result=await productService.GetAll();
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Searches for products whose names contain or match the specified word.
        /// </summary>
        /// <remarks>The search is case-insensitive. If the search operation encounters an error, a 500
        /// status code is returned with an error message.</remarks>
        /// <param name="word">The word to search for within product names. Must consist of alphabetic characters only.</param>
        /// <returns>An <see cref="ActionResult"/> containing a list of products with names that match or contain the specified
        /// word, or an error message if the search fails.</returns>
        [HttpGet("{word:alpha}")]
        public async Task<ActionResult> Search(string word)
        {
            var result =await productService.Search(word);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to find a product by its ID. If the
        /// product is not found, a 404 Not Found response is returned with a message indicating the product was not
        /// found.</remarks>
        /// <param name="id">The unique identifier of the product to retrieve. Must be a positive integer.</param>
        /// <returns>An <see cref="ActionResult"/> containing the product if found; otherwise, a 404 Not Found response.</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result =await productService.GetById(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Adds a new product to the system.
        /// </summary>
        /// <remarks>This method maps the provided <paramref name="productDto"/> to a <see
        /// cref="Product"/> entity and attempts to add it to the database. It uses the unit of work pattern to ensure
        /// that the operation is completed successfully.</remarks>
        /// <param name="productDto">The data transfer object containing the product details to be added.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation. Returns <see cref="BadRequest"/> if
        /// the addition fails, or <see cref="Ok"/> with the added product details if successful.</returns>
        [HttpPost]
        public async Task<ActionResult> AddProduct(ProductDto productDto)
        {
            var result =await productService.AddProduct(productDto);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Updates an existing product with the specified details.
        /// </summary>
        /// <param name="productDto">The data transfer object containing the updated product details. Must include a valid product ID.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the update operation. Returns <see cref="NotFound"/>
        /// if the product is not found, <see cref="BadRequest"/> if the update fails, or <see cref="Ok"/> with the
        /// updated product on success.</returns>
        [HttpPut]
        public async Task<ActionResult> Update(UpdateProductDto productDto)
        {
            var result =await productService.Update(productDto);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Deletes the product with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the product to delete. Must be a positive integer.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.  Returns <see cref="NotFound"/> if the
        /// product with the specified ID does not exist. Returns <see cref="BadRequest"/> if the deletion operation
        /// fails. Returns <see cref="NoContent"/> if the product is successfully deleted.</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result=await productService.Delete(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return NoContent();
        }


    }
}

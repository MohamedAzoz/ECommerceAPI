using AutoMapper;
using ECommerce.Core.DTOs.Category;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        //GET    /api/categories
        //GET    /api/categories/{id}
        //POST   /api/categories (Admin)
        //PUT    /api/categories/{id} (Admin)
        //DELETE /api/categories/{id} (Admin)
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }
       

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result =await categoryService.GetAll();
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result = await categoryService.GetById(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }

            return Ok(result.Value);
           
        }

        
        [HttpPost]
        public async Task<ActionResult> AddCategory(CreateCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
             var result = await categoryService.AddCategory(categoryDto);

            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Created();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await categoryService.Delete(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }

            return NoContent();
        }

    }
}

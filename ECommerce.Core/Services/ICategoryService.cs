using ECommerce.Core.DTOs.Category;
using ECommerce.Core.Entities;
using ECommerce.Core.Result_Pattern;

namespace ECommerce.Core.Services
{
    public interface ICategoryService
    {
        public Task<Result<ICollection<Category>>> GetAll();
        public Task<Result<CategoryDto>> GetById(int id);
        public Task<Result> AddCategory(CreateCategoryDto categoryDto);
        public Task<Result> Delete(int id);
    }
}

using AutoMapper;
using ECommerce.Core.DTOs.Category;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;

namespace ECommerce.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CategoryService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

       
        public async Task<Result<ICollection<Category>>> GetAll()
        {
            var result = await unitOfWork.Categories.GetAllAsync();
            if (!result.IsSuccess)
            {
                return Result<ICollection<Category>>.Failure("An error occurred while retrieving Categories.",500);
            }
            
            //var categoryDto = mapper.Map<ICollection<CategoryDto>>(result.Value);
            return Result<ICollection<Category>>.Success(result.Value);
        }


        public async Task<Result<CategoryDto>> GetById(int id)
        {
            var result = await unitOfWork.Categories.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                return Result<CategoryDto>.Failure($"Category with ID {id} not found.",500);
            }
            var category = mapper.Map<CategoryDto>(result.Value);
            return Result<CategoryDto>.Success(category);
        }


        public async Task<Result> AddCategory(CreateCategoryDto categoryDto)
        {
            var existingCategory = await unitOfWork.Categories
                 .FindAsync(c => c.Name.ToLower() == categoryDto.Name.ToLower());

            if (existingCategory.IsSuccess)
            {
                return Result.Failure("A category with this name already exists.", 500);
            }

            Category category = mapper.Map<Category>(categoryDto);
            var AddResult = await unitOfWork.Categories.AddAsync(category);
            if (!AddResult.IsSuccess)
            {
                return Result.Failure(AddResult.Error);
            }
            await unitOfWork.Completed();
            return Result.Success();
        }


        public async Task<Result> Delete(int id)
        {
            var result = await unitOfWork.Categories.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                return Result.Failure($"Category with ID {id} not found.", 500);
            }
            var DeleteResult = unitOfWork.Categories.Delete(result.Value);
            if (!DeleteResult.IsSuccess)
            {
                return Result.Failure(DeleteResult.Error);
            }

            await unitOfWork.Completed();
            return Result.Success();
        }

    }
}
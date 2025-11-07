using AutoMapper;
using ECommerce.Core.DTOs.Product;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;

namespace ECommerce.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        
        public async Task<Result<ICollection<Product>>> GetAll()
        {
            var result = await unitOfWork.Products.GetAllAsync();
            if (!result.IsSuccess)
            {
                return Result<ICollection<Product>>.Failure("An error occurred while retrieving Orders.",500);
            }
            return Result<ICollection<Product>>.Success(result.Value);
        }

        public async Task<Result<ICollection<Product>>> Search(string word)
        {
            var result = await unitOfWork.Products.FindAllAsync((x) => x.Name.ToLower().Contains(word.ToLower())
                                || x.Name.ToLower() == word.ToLower());
            if (!result.IsSuccess)
            {
                return Result<ICollection<Product>>.Failure(result.Error ?? "An error occurred during the search.", 500);
            }
            return Result<ICollection<Product>>.Success(result.Value);
        }
        
        public async Task<Result<Product>> GetById(int id)
        {
            var result = await unitOfWork.Products.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                return Result<Product>.Failure($"Product with ID {id} not found.", 404 );
            }
            return Result<Product>.Success(result.Value);
        }
        
        public async Task<Result<Product>> AddProduct(ProductDto productDto)
        {
            Product product = mapper.Map<Product>(productDto);
            var result = await unitOfWork.Products.AddAsync(product);
            if (!result.IsSuccess)
            {
                return Result<Product>.Failure(result.Error);
            }
            await unitOfWork.Completed();
            return Result<Product>.Success(result.Value);
        }
        
        public async Task<Result<Product>> Update(UpdateProductDto productDto)
        {
            var productUpdat = mapper.Map<Product>(productDto);

            var result = await unitOfWork.Products.FindAsync((x) =>
                                        x.Id == productUpdat.Id);
            if (!result.IsSuccess)
            {
                return Result<Product>.Failure($"Product with ID {productDto.Id} not found.", 404);
            }

            var resultUpdate = unitOfWork.Products.Update(result.Value);
            if (!resultUpdate.IsSuccess)
            {
                return Result<Product>.Failure(resultUpdate.Error);
            }
            await unitOfWork.Completed();
            return Result<Product>.Success(resultUpdate.Value);
        }

        public async Task<Result> Delete(int id)
        {
            var result = await unitOfWork.Products.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                return Result.Failure($"Product with ID {id} not found.", 404);
            }
            var DeleteResult = unitOfWork.Products.Delete(result.Value);
            if (!DeleteResult.IsSuccess)
            {
                return Result.Failure(DeleteResult.Error);
            }
            await unitOfWork.Completed();
            return Result.Success();
        }

    }
}
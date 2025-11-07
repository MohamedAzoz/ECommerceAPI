using ECommerce.Core.DTOs.Product;
using ECommerce.Core.Entities;
using ECommerce.Core.Result_Pattern;

namespace ECommerce.Core.Services
{
    public interface IProductService
    {
        public Task<Result<ICollection<Product>>> GetAll();
        public Task<Result<ICollection<Product>>> Search(string word);
        public Task<Result<Product>> GetById(int id);
        public Task<Result<Product>> AddProduct(ProductDto productDto);
        public Task<Result<Product>> Update(UpdateProductDto productDto);
        public Task<Result> Delete(int id);
    }
}

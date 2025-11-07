using ECommerce.Core.DTOs.Cart;
using ECommerce.Core.Result_Pattern;

namespace ECommerce.Core.Services
{
    public interface ICartService
    {
        public Task<Result<ICollection<CartItemDto>>> ViewCart(string userId);
        public Task<Result> AddToCart(AddToCartDto addToCartDto);
        public Task<Result> IncresItem(int id);
        public Task<Result> DeleteItem(int id);
        public Task<Result> DecriseItem(int id);
        public Task<Result> ClearItems(string userId);
    }
}

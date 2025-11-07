using ECommerce.Core.Entities;
using ECommerce.Core.Result_Pattern;
using System.Linq.Expressions;

namespace ECommerce.Core.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        public Task<Result<CartItem>> AddToCart(CartItem cartItem);
        public Task<Result<Cart>> GetWithIncludeAsync(
                  Expression<Func<Cart, bool>> expression);
    }
}

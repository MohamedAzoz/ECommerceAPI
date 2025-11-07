using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Result_Pattern;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly AppDbContext context;

        public CartRepository(AppDbContext _context) : base(_context)
        {
            context = _context;
        }

        public async Task<Result<CartItem>> AddToCart(CartItem cartItem)
        {
            if (cartItem == null)
            {
                return Result<CartItem>.Failure("Cannot add a null item.");
            }
            await context.AddAsync(cartItem); // استخدام Async
            return Result<CartItem>.Success(cartItem);
        }

        public async Task<Result<Cart>> GetWithIncludeAsync(Expression<Func<Cart, bool>> expression)
        {
            //IQueryable<Cart> query = context.Carts.;

            // تطبيق الـ Includes (التحميل المتضمن)
            //foreach (var include in includes)
            //{
            //    query = query.Include(include);
            //}

            // البحث عن العنصر وتطبيق الشرط
            var query = context.Carts.Include(x=>x.CartItems);
            var item=query.FirstOrDefault(expression);
            if (item == null)
            {
                return Result<Cart>.Failure("Item not found matching the criteria.");
            }

            return Result<Cart>.Success(item);
        }

    }
}

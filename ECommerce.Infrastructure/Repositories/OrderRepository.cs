using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext context;

        public OrderRepository(AppDbContext _context) : base(_context)
        {
            context = _context;
        }
    }
}

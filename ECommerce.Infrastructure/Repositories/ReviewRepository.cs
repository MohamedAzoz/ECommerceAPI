using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly AppDbContext context;

        public ReviewRepository(AppDbContext _context) : base(_context)
        {
            context = _context;
        }
    }
}

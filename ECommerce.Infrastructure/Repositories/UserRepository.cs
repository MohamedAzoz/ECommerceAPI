using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext _context) : base(_context)
        {
            context = _context;
        }
    }
}

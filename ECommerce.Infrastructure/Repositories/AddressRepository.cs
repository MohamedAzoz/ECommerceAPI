using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(AppDbContext _context) : base(_context)
        {
        }
    }
}

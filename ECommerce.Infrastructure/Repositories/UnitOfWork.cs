using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;
        public UnitOfWork(AppDbContext _context)
        {
            context = _context;
            Carts=new CartRepository(_context);
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            Orders = new OrderRepository(_context);
            Reviews = new ReviewRepository(_context);
            Users = new UserRepository(_context);
            Addresses=new AddressRepository(_context);
            CartItems=new GenericRepository<CartItem>(_context);
            OrderItems= new GenericRepository<OrderItem>(_context);
        }

        public ICartRepository Carts { get;private set; }
        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IUserRepository Users { get; private set; }
        public IAddressRepository Addresses { get; private set; }

        public IGenericRepository<CartItem> CartItems { get; private set; }

        public IGenericRepository<OrderItem> OrderItems { get; private set; }

        public async Task<bool> Completed()
        {
            return (await context.SaveChangesAsync()) > 0;
        }
    }
}

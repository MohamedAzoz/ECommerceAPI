using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IUnitOfWork
    {
        public ICartRepository Carts { get; }
        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public IReviewRepository Reviews { get; }
        public IUserRepository Users { get; }
        public IAddressRepository Addresses { get; }
        public IGenericRepository<CartItem> CartItems { get; }
        public IGenericRepository<OrderItem> OrderItems { get; }
        public Task<bool> Completed();
    }
}

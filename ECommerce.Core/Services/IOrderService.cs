using ECommerce.Core.DTOs.Order;
using ECommerce.Core.Entities;
using ECommerce.Core.Result_Pattern;

namespace ECommerce.Core.Services
{
    public interface IOrderService
    {
        public Task<Result<ICollection<OrderDto>>> GetByUserId(string userId);
        public Task<Result<ICollection<OrderItem>>> GetById(int id);
        public Task<Result<ICollection<OrderDto>>> GetAll();
        public Task<Result> NewOrder(CreateOrderDto orderDto, string userId);
        public Task<Result<Order>> UpdateStatus(UpdateOrderStatusDto orderStatusDto);
        public Task<Result> Delete(int id);
    }
}

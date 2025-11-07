using AutoMapper;
using ECommerce.Core.DTOs.Order;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;

namespace ECommerce.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public OrderService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }


        public async Task<Result<ICollection<OrderDto>>> GetAll()
        {
            var result = await unitOfWork.Orders.GetAllAsync();
            if (!result.IsSuccess)
            {
                return Result<ICollection<OrderDto>>.Failure("An error occurred while retrieving Orders.",500);
            }
            var orderDto = mapper.Map<ICollection<OrderDto>>(result.Value);
            return Result<ICollection<OrderDto>>.Success(orderDto);
        }
        public async Task<Result<ICollection<OrderDto>>> GetByUserId(string userId)
        {
            var result = await unitOfWork.Orders.FindAllAsync((o) => o.UserId == userId);
            if (!result.IsSuccess)
            {
                return Result<ICollection<OrderDto>>.Failure($"Orders with UserId {userId} not found.",404 );
            }
            var orderDto = mapper.Map<ICollection<OrderDto>>(result.Value);
            return Result<ICollection<OrderDto>>.Success(orderDto);
        }
        public async Task<Result<ICollection<OrderItem>>> GetById(int id)
        {
            var result = await unitOfWork.Orders.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                return Result<ICollection<OrderItem>>.Failure($"Orders with ID {id} not found." , 404);
            }
            var OrderItems = await unitOfWork.OrderItems.FindAllAsync((x) => x.OrderId == id);
            if (!OrderItems.IsSuccess)
            {
                return Result<ICollection<OrderItem>>.Success(new List<OrderItem>());
            }

            return Result<ICollection<OrderItem>>.Success(OrderItems.Value);
        }
        public async Task<Result> NewOrder(CreateOrderDto orderDto,string userId)
        {
            Order order = mapper.Map<Order>(orderDto);

            var cartresult = await unitOfWork.Carts.FindAsync(x => x.UserId == userId);
            if (!cartresult.IsSuccess)
            {
                return Result.Failure("Cart not found.",404);
            }

            var cartItemsResult = await unitOfWork.CartItems.FindAllWithIncludeAsync(
                    x => x.CartId == cartresult.Value.Id,
                    x => x.Product!
                );

            if (!cartItemsResult.IsSuccess || !cartItemsResult.Value.Any())
            {
                // سلة فارغة (خطأ في المنطق إذا كانت السلة تسمح بأن تكون فارغة عند إنشاء طلب)
                return Result.Failure("Cart is empty or items not found.", 400);
            }

            decimal TotalAmount = 0;
            var productsToUpdate = new List<Product>();
            foreach (var item in cartItemsResult.Value)
            {
                var product = item.Product;
                if (product.StockQuantity < item.Quantity)
                {
                    return Result.Failure($"Insufficient stock for product {product.Name}. Available: {product.StockQuantity}", 400);
                }

                var orderItem = mapper.Map<OrderItem>(item); // تحويل CartItem إلى OrderItem
                orderItem.OrderId = order.Id;
                orderItem.Order = order;

                TotalAmount += item.Quantity * orderItem.Price;

                await unitOfWork.OrderItems.AddAsync(orderItem);

                product.StockQuantity -= item.Quantity;
                productsToUpdate.Add(product);
            }
            
            foreach (var item in productsToUpdate)
            {
               var resultProduct= unitOfWork.Products.Update(item);
                if (!resultProduct.IsSuccess)
                {
                   return Result.Failure(resultProduct.Error, 500);
                }
            }

            order.TotalAmount = TotalAmount;
            var result = await unitOfWork.Orders.AddAsync(order);
            if (!result.IsSuccess)
            {
                return Result.Failure(result.Error,500);
            }
            
            var resultDalete=await unitOfWork.CartItems.ClearAsync(x=>x.CartId == cartresult.Value.Id);
            if (!resultDalete.IsSuccess)
            {
                return Result.Failure(resultDalete.Error,500);
            }

            await unitOfWork.Completed();

            return Result.Success();
        }
        public async Task<Result<Order>> UpdateStatus(UpdateOrderStatusDto orderStatusDto)
        {
            var found=await unitOfWork.Orders.FindAsync(x=>x.Id == orderStatusDto.Id);
            if (found.IsFailure)
            {
                return Result<Order>.Failure(found.Error, 404);
            }
            Order order = found.Value;
            order.Status = orderStatusDto.Status;
            var result = unitOfWork.Orders.Update(order);
            if (result.IsFailure)
            {
                return Result<Order>.Failure(result.Error,500);
            }
            await unitOfWork.Completed();
            return Result<Order>.Success(result.Value);
        }
        public async Task<Result> Delete(int id)
        {
            var result = await unitOfWork.Orders.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                return Result.Failure($"Order with ID {id} not found.", 404);
            }
            else
            {
                var clearResult = await unitOfWork.OrderItems.ClearAsync(x => x.OrderId == id);
                if (!clearResult.IsSuccess)
                {
                    return Result.Failure(clearResult.Error,500);
                }
                var DeleteResult = unitOfWork.Orders.Delete(result.Value);
                if (!DeleteResult.IsSuccess)
                {
                    return Result.Failure(DeleteResult.Error,500);
                }
            }

            await unitOfWork.Completed();
            return Result.Success();
        }

    }
}


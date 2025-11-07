using AutoMapper;
using ECommerce.Core.DTOs.Cart;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;

namespace ECommerce.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CartService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

        public async Task<Result<ICollection<CartItemDto>>> ViewCart(string userId)
        {
            var userResult = await unitOfWork.Users.FindAsync(x => x.Id == userId);
            if (!userResult.IsSuccess || userResult.Value.CartId == null)
            {
                return Result<ICollection<CartItemDto>>.Failure( "User or associated cart not found.",404);
            }
            var cartId = userResult.Value.CartId;
            // 3. جلب عناصر السلة *مع تضمين تفاصيل المنتج*
            var itemsResult = await unitOfWork.CartItems.FindAllWithIncludeAsync(
                x => x.CartId == cartId,
                x => x.Product! // <=== هنا يتم تضمين بيانات المنتج
            );

            if (!itemsResult.IsSuccess)
            {
                if (itemsResult.Error == "No items found matching the criteria.")
                {
                    return Result<ICollection<CartItemDto>>.Success(new List<CartItemDto>()); // إرجاع قائمة فارغة (سلة فارغة)
                }

                return Result<ICollection<CartItemDto>>.Failure("An error occurred while retrieving cart items with products.",500);
            }

            var cartItemDtos = mapper.Map<ICollection<CartItemDto>>(itemsResult.Value);
            return Result<ICollection<CartItemDto>>.Success(cartItemDtos);

        }


        public async Task<Result> AddToCart(AddToCartDto addToCartDto) // <==
        {
            var productResult = await unitOfWork.Products.FindAsync(x => x.Id == addToCartDto.ProductId);
            if (!productResult.IsSuccess)
            {
                return Result.Failure("Product not found." ,404);
            }
            if (productResult.Value.StockQuantity < addToCartDto.Quantity)
            {
                return Result.Failure("Quantity over the stock",400);
            }

            // 2. التأكد من وجود السلة
            var cartResult = await unitOfWork.Carts.FindAsync(x => x.Id == addToCartDto.CartId);
            if (!cartResult.IsSuccess)
            {
                return Result.Failure("Cart not found.", 404);
            }

            var existingItemResult = await unitOfWork.CartItems.FindAsync(
                x => x.CartId == addToCartDto.CartId && x.ProductId == addToCartDto.ProductId);

            if (existingItemResult.IsSuccess)
            {
                var existingItem = existingItemResult.Value;
                existingItem.Quantity += addToCartDto.Quantity;

                var updateResult = unitOfWork.CartItems.Update(existingItem);
                if (!updateResult.IsSuccess)
                {
                    return Result.Failure(updateResult.Error);
                }
            }
            else
            {
                CartItem cartItem = mapper.Map<CartItem>(addToCartDto);
                cartItem.Product = productResult.Value; 

                var addResult = await unitOfWork.CartItems.AddAsync(cartItem);
                if (!addResult.IsSuccess)
                {
                    return Result.Failure(addResult.Error);
                }
            }

            // 4. الحفظ النهائي والتأكيد
            await unitOfWork.Completed();
            return Result.Success();
            // return Result.Success("Item added/updated successfully.");
        }


        public async Task<Result> IncresItem(int id)
        {
            var result = await unitOfWork.CartItems.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                return Result.Failure($"CartItem with ID {id} not found.",404 );
            }
            var cartItem = result.Value;
            var productResult = await unitOfWork.Products.FindAsync(x => x.Id == cartItem.ProductId);
            if (!productResult.IsSuccess)
            {
                return Result.Failure("Product not found",404);
            }
            var Value = cartItem.Quantity;
            Value += 1;
            cartItem.Quantity = Value;

            if (Value > productResult.Value.StockQuantity)
            {
                return Result.Failure("Quantity Stock is Empty.", 400);
            }
            var DeleteResult = unitOfWork.CartItems.Update(cartItem);
            if (!DeleteResult.IsSuccess)
            {
                return Result.Failure(DeleteResult.Error);
            }
            await unitOfWork.Completed();
            return Result.Success();
        }


        public async Task<Result> DeleteItem(int id)
        {
            var result = await unitOfWork.CartItems.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                return Result.Failure($"CartItem with ID {id} not found.",404);
            }

            var DeleteResult = unitOfWork.CartItems.Delete(result.Value);
            if (!DeleteResult.IsSuccess)
            {
                return Result.Failure(DeleteResult.Error);
            }
            await unitOfWork.Completed();
            return Result.Success();
        }


        public async Task<Result> DecriseItem(int id)
        {
            var result = await unitOfWork.CartItems.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                return Result.Failure($"CartItem with ID {id} not found.", 404);
            }
            var cartItem = result.Value;
            var Value = cartItem.Quantity;
            Value -= 1;
            cartItem.Quantity = Value;
            if (Value == 0)
            {
                return await DeleteItem(id);
            }
            var DeleteResult = unitOfWork.CartItems.Update(cartItem);
            if (!DeleteResult.IsSuccess)
            {
                return Result.Failure(DeleteResult.Error);
            }
            await unitOfWork.Completed();
            return Result.Success();
        }


        public async Task<Result> ClearItems(string userId)
        {

            var resultUser = await unitOfWork.Users.FindAsync(x => x.Id == userId);
            if (!resultUser.IsSuccess)
            {
                // استخدام NotFound في حال عدم العثور على السلة هو الأفضل (HTTP 404)
                return Result.Failure("Cart not found.", 404);
            }
            var cartId = resultUser.Value.CartId;
            var result = await unitOfWork.CartItems.ClearAsync((x) => x.CartId == cartId);
            if (!result.IsSuccess)
            {
                return Result.Failure($"Cart with ID {cartId} not found.", 404);
            }
            await unitOfWork.Completed();
            return Result.Success();
        }

    }
}

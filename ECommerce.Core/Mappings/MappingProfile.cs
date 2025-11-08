using AutoMapper;
using ECommerce.Core.DTOs.Address;
using ECommerce.Core.DTOs.Auth;
using ECommerce.Core.DTOs.Cart;
using ECommerce.Core.DTOs.Category;
using ECommerce.Core.DTOs.Order;
using ECommerce.Core.DTOs.Product;
using ECommerce.Core.DTOs.Review;
using ECommerce.Core.Entities;
using ECommerce.Core.Models;

namespace ECommerce.Core.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LoginDto, AppUser>()
                .ForMember(dis => dis.PasswordHash, src => src.MapFrom(s => s.Password))
                .ForMember(dis => dis.Email, src => src.MapFrom(s => s.Email))
                .ReverseMap();

           
            CreateMap<RegisterModel, AppUser>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<ProductDto, Product>()
                .ReverseMap();
            
            //CreateMap<CartDto, Cart>()
            //   .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItems))
            //   .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AddToCartDto, CartItem>()
               .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
               .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ReverseMap();
            CreateMap<CartItemDto, CartItem>()
               .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.CartId))
               .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
               .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<CartItem , OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
               .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ReverseMap();

            CreateMap<UpdateProductDto, Product>()
                .ReverseMap();

            CreateMap<AddressDto, Address>()
                .ReverseMap();

            CreateMap<ReviewDto, Review>()
                .ReverseMap();

            CreateMap<ReviewListDto, Review>()
                .ReverseMap();

            CreateMap<CategoryDto, Category>()
                .ReverseMap();

            CreateMap<CreateCategoryDto, Category>()
                .ReverseMap();
           
            CreateMap<OrderDto, Order>()
                .ReverseMap();

            CreateMap<CreateOrderDto, Order>()
                .ReverseMap();
        }

    }
}

using ECommerce.Core.DTOs.Product;

namespace ECommerce.Core.DTOs.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; }

        // استخدم DTO المنتج بدلاً من Entity Model
        public ProductDto Product { get; set; }
    }
}

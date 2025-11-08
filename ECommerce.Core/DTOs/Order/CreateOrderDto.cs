using ECommerce.Core.Entities;

namespace ECommerce.Core.DTOs.Order
{
    public class CreateOrderDto
    {
        //public string UserId { get; set; }
        public string Status { get; set; } // 
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int ShippingAddressId { get; set; }
        public string PaymentMethod { get; set; }
        //public ICollection<OrderItem> OrderItems { get; set; }
    }
}

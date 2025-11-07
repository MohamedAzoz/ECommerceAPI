using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // 
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int ShippingAddressId { get; set; }
        public string PaymentMethod { get; set; }

        // Navigation Properties
        public AppUser? User { get; set; }
        public Address? ShippingAddress { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}

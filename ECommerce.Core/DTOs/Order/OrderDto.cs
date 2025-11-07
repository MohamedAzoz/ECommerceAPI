namespace ECommerce.Core.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // 
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int ShippingAddressId { get; set; }
        public string PaymentMethod { get; set; }
    }
}

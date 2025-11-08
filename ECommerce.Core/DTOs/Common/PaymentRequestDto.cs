namespace ECommerce.Core.DTOs.Common
{
    public class PaymentRequestDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string UserEmail { get; set; }
    }
}

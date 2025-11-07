using ECommerce.Core.Enums;

namespace ECommerce.Core.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}

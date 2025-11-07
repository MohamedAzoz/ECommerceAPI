namespace ECommerce.Core.DTOs.Product
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
    }
}

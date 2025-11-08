using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.DTOs.Product
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        //public string ImageUrl { get; set; }
       
    }
}

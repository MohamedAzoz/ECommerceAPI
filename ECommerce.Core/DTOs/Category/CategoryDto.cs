namespace ECommerce.Core.DTOs.Category
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}

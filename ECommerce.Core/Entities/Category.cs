namespace ECommerce.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; } // Delete
        public int? ParentCategoryId { get; set; } // Delete

        // Navigation Properties
        public Category? ParentCategory { get; set; } // Delete
        public ICollection<Category>? SubCategories { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}

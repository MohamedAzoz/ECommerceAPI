namespace ECommerce.Core.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string? StoredImageName { get; set; }
        public string ContentType { get; set; }
        public string AltText { get; set; }
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
        public int ProductId { get; set; }

        // Navigation Property
        public Product? Product { get; set; }
    }
}

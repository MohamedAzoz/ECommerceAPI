namespace ECommerce.Core.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public AppUser User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}

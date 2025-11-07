namespace ECommerce.Core.DTOs.Review
{
    public class ReviewListDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; } // 1 to 5
        public string Title { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public bool IsApproved { get; set; }
    }
}

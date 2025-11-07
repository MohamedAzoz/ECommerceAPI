using Microsoft.AspNetCore.Identity;

namespace ECommerce.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public string? EmailVerificationCode { get; set; }
        public DateTime? EmailVerificationCodeExpiry { get; set; }
        public int? CartId { get; set; }  //< ===== >

        // Navigation Properties
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public Cart? Cart { get; set; } 
    }
}

using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs.Auth
{
    public class RevokeToken
    {
        [Required]
        public string Token { get; set; }
    }
}

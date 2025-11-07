using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Models
{
    public class VerifyModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Please enter a valid Gmail address.")]
        public string Email { get; set; }
        [Required]
        [StringLength(6)]
        public string Code { get; set; }
    }
}

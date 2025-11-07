using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, one digit and one special character")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        [Display(Name = "Password Confirmed")]
        public string ConfirmPassword { get; set; }
    }
}

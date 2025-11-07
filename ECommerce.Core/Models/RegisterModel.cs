using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Models
{
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Please enter a valid Gmail address.")]
        public string Email { get; set; }

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

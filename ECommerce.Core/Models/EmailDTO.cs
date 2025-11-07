using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Models
{
    public class EmailDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Please enter a valid Gmail address.")]
        public string Email { get; set; }
        [Required]
        public string Page { get; set; }
        public string? LinkURL { get; set; }=null;
        [Required]
        public string ClientType { get; set; }

        public string? Scheme { get; set; }=null;

    }
}

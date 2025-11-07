using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.DTOs.Auth
{
    public class RoleDto
    {
        [Required]
        public string RoleName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class UsersInRoles
    {
        [Required]
        public string RoleName { get; set; }
        [Required]
        public string Email { get; set; }
    }
}

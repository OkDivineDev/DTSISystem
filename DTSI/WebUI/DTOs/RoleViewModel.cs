using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class RoleViewModel
    {
        public string? Id { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}

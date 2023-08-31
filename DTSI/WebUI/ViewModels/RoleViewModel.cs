using System.ComponentModel.DataAnnotations;

namespace WebUI.ViewModels
{
    public class RoleViewModel
    {
        public string? Id { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}

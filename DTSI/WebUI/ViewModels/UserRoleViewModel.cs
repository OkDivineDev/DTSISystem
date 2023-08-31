using System.ComponentModel.DataAnnotations;

namespace WebUI.ViewModels
{
    public class UserRoleViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string RoleId { get; set; }
    }
}

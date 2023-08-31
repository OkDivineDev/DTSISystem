using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUI.DTOs
{
    public class LecturerVm
    {
        public string? Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Name { get; set; }

       
        public string? Email { get; set; }

        [Display(Name = "Department")]
        public string? DepartmentID { get; set; }



        
    }
}
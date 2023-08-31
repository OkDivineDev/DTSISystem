using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class LecturerUploadVM
    {
        [Display(Name = "Department")]
        public string  DepartmentId { get; set; }
        [Display(Name ="Employee List")]
        [Required(ErrorMessage ="*")]
        public IFormFile EmployeeFile { get; set; }
    }
}
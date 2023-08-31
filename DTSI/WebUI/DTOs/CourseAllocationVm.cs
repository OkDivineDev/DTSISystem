using DataAccessLayer.Enum;
using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class CourseAllocationVm
    {
        public string? Id { get; set; }
        [Required(ErrorMessage ="No Course is selected!")]
        public string CourseID { get; set; }

        [Required(ErrorMessage = "Employee is null!")]
        public string LecturerID { get; set; }

        [Required(ErrorMessage ="No Semester is selected!")]
        public SemesterEnum Semester { get; set; }
         
        [Required(ErrorMessage ="No Session is selected!")]
        public string Session { get; set; }

        
        public string? DepartmentID { get; set; }

        public bool Approved { get; set; }
    }
}
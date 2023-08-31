using DataAccessLayer.Enum;
using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class CourseAllocationApprovalVm
    {
        [Required(ErrorMessage = "Session is null!")]
        public string Session { get; set; }

        [Required(ErrorMessage = "Semester is null!")]
        public SemesterEnum Semester { get; set; }
    }
}

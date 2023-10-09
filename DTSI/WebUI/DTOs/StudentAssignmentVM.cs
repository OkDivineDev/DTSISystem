
using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class StudentAssignmentVM
    {
        public string? Id { get; set; }

        public string StudentRegNo { get; set; }

        public string? DateSubmitted { get; set; }

        public string AssignmentID { get; set; }

        [Required(ErrorMessage = "Answer is REQUIRED")]
        [RegularExpression(@"^[\w\-'’“”:;?\+*%/&`(),.0-9\s]+$",
         ErrorMessage = "Not in proper format!")]
        [DataType(DataType.MultilineText)]
        public string Answer { get; set; }

        public IFormFile? Image { get; set; }
    }
}
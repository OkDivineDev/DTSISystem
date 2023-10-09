using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class SearchAllocationVm
    {
        [Display(Name = "Academic Session")]
        public string? _Session { get; set; }
    }

    public class SearchAssignmentVm
    {
        [Display(Name = "Assignment Code")]
        [StringLength(16)]
        [RegularExpression(@"^[\w]+$",
         ErrorMessage = "Not in proper format!")]
        public string? Code { get; set; }
    }
}

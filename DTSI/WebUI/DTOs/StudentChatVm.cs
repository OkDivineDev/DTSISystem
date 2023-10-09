using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class StudentChatVm
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Course is REQUIRED")]
        [Display(Name = "Course")]
        public string CourseId { get; set; }

        [Display(Name = "Outline")]
        [Required(ErrorMessage = "Outline is REQUIRED")]
        public string OutlineId { get; set; }

        [Required(ErrorMessage = "Message is REQUIRED")]
        [RegularExpression(@"^[\w\-'’“”:;?/&`(),.0-9\s]+$",
         ErrorMessage = "Not in proper format!")]
        public string Message { get; set; }

        public IFormFile? Image { get; set; }
    }
}

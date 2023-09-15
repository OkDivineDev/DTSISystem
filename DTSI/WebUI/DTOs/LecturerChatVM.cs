using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class LecturerChatVM
    {
        public string? Id { get; set; }


        [Display(Name = "Chat")]
        public string? ChatId { get; set; }

        
        [Display(Name = "Course")]
        public string? CourseId { get; set; }

        [Display(Name = "Outline")]
        public string? OutlineId { get; set; }

        [Required(ErrorMessage = "Message is REQUIRED")]
        [RegularExpression(@"^[\w\-'’“”&`(),.0-9\s]+$",
         ErrorMessage = "Not in proper format!")]
        public string Message { get; set; }

        public IFormFile? Image { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class AcademicSessionVM
    {
        [Required(ErrorMessage = "Please provide the session, e.g 2023/2024")]
        [RegularExpression(@"[0-9]{4}[\/]{1}[0-9]{4}$",
         ErrorMessage = "Not in proper format!")]
        [StringLength(9, ErrorMessage ="Not more than 9 characters!")]
        public string Title { get; set; }

        public string? Id { get; set; }

    }
}

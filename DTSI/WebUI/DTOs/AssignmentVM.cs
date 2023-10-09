using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class AssignmentVM
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Course is REQUIRED")]
        public string CourseID { get; set; }

        [Required(ErrorMessage = "Question is REQUIRED")]
        [RegularExpression(@"^[\w\-'’“”:;?/&`(),.0-9\s]+$",
         ErrorMessage = "Not in proper format!")]
        [DataType(DataType.MultilineText)]
        public string Questions { get; set; }
        [StringLength(16)]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Lecturer is REQUIRED")]
        public string LecturerID { get; set; }

        public List<string>? QuestionList { get; set; }

        public string? Date { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? SubmissionDate { get; set; }
    }
}

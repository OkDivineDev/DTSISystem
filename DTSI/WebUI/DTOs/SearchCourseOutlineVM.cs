using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class SearchCourseOutlineVM
    {
        [Required(ErrorMessage = "Course is requred!")]
        [Display(Name = "Course")]
        public string? CourseID { get; set; }  
    }
}

using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class SemesterViewModel
    {
        public int ID { get; set; }
        [Display(Name = "Course Code")]
        public string CourseID { get; set; }
        public string Code { get; set; }
        public string LetterID { get; set; }
        public int Level { get; set; }
        public int Semester { get; set; }

    }
}
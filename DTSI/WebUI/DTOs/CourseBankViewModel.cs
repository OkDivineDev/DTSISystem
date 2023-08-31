

using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class CourseBankViewModel
    {
        public string? ID { get; set; }
        public string? DepartmentID { get; set; }
        public string? Department { get; set; }

        [Required(ErrorMessage = "*")]
        public string Code { get; set; }
        [Required(ErrorMessage = "*")]
        public string Title { get; set; }
        [Required(ErrorMessage = "*")]
        public int Unit { get; set; }

    }
}
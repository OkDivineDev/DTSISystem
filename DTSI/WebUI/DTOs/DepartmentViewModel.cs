
using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class DepartmentViewModel 
    {
      
        public string? Id { get; set; }


        [Required(ErrorMessage = "Name is REQUIRED")]
        [RegularExpression(@"^[\w\-'’“”&`(),.0-9\s]+$",
         ErrorMessage = "Not in proper format!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Code is REQUIRED")]
        [RegularExpression(@"^[\w\-'’“”&`(),.0-9\s]+$",
         ErrorMessage = "Not in proper format!")]
        [StringLength(7, ErrorMessage ="Not more than 7 characters!")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Hod Email is REQUIRED")]
        public string HODEmail { get; set; }

        [Required(ErrorMessage = "Hod Name is REQUIRED")]
        [RegularExpression(@"^[\w\-'’“”&`(),.0-9\s]+$",
         ErrorMessage = "Not in proper format!")]
        public string HODName { get; set; }
    }
}

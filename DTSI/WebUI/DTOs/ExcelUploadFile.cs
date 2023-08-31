using DataAccessLayer.Enum;
using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class ExcelUploadFile
    {
        [Display(Name = "Excel File")]
        [Required(ErrorMessage ="Excel File is required")]
        public IFormFile ExcelFile { get; set; }


        public SemesterEnum? Semester { get; set; }

     
        public string? Session { get; set; }

    }
}

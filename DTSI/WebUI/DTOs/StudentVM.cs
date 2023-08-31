
using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class StudentVM
    {
        public string RegNo { get; set; }
        public string Name { get; set; }
        public string EntryYear { get; set; }
        public string GraduationYear { get; set; }
        public string EntryMode { get; set; }
        [Required(ErrorMessage = "Jamb Reg No is required!")]
        public string JambRegNo { get; set; }
        [Required(ErrorMessage = "Gender is required!")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Level is required!")]
        public int Level { get; set; }
        public string Passport { get; set; }
        public IFormFile PassportUrl { get; set; }

        //ACOUNT DETAILS
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Institution")]
        public int InstitutionID { get; set; }

    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Student
    {
        [Key]
        [StringLength(13)]
        [Required(ErrorMessage = "Reg No is required!")]
        public string RegNo { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Entry Year is required!")]
        public int EntryYear { get; set; }
        [Required(ErrorMessage = "Graduatio Year is required!")]
        public int GraduationYear { get; set; }
        [Required(ErrorMessage = "Entry Mode is required!")]
        public string EntryMode { get; set; }
        [Required(ErrorMessage = "Jamb Reg No is required!")]
        public string JambRegNo { get; set; }
        [Required(ErrorMessage = "Level is required!")]
        public int Level { get; set; }
        public string Passport { get; set; }
        public string Gender { get; set; }

        [Display(Name = "Department")]
        [ForeignKey("Department")]
        public string DepartmentID { get; set; }


        public virtual Assignment Assignment { get; set; }
        public virtual Department Department { get; set; }
    }
}
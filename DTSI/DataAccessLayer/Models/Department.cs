using DataAccessLayer.GeneralModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Department : BaseEntity
    {


        
        public string Name { get; set; }
        public string Code { get; set; }


        [Display(Name ="HOD Email")]
        public string HODUserId { get; set; }

        [NotMapped]
        [DataType(DataType.EmailAddress)]
        public string HODEmail { get; set; }

        [NotMapped]
        [Display(Name ="HOD Full Name")]
        public string HODName { get; set; }


        public virtual ICollection<Lecturer>? Lecturers { get; set; }
        public virtual ICollection<Student>? Students { get; set; } 
        public virtual ICollection<Admission>? Admissions { get; set; } 
        public virtual ICollection<CourseBank>? Courses { get; set; } 
        public virtual ICollection<CourseAllocation>? CourseAllocations { get; set; }

    }
}
using DataAccessLayer.GeneralModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class CourseAllocation : BaseEntity
    {
        [ForeignKey("Course")]
        public string CourseID { get; set; }

        [ForeignKey("Lecturer")]
        public string LecturerID { get; set; }

        [ForeignKey("Depatment")]
        public string DepartmentID { get; set; }

        public int Semester { get; set; }
        public string Session { get; set; }

        public bool Approved { get; set; }

        public virtual Lecturer Lecturer { get; set; }
        public virtual CourseBank Course { get; set; }

    }
}
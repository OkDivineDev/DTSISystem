using DataAccessLayer.GeneralModel;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class Assignment : BaseEntity
    {
        public string CourseID { get; set; }

        [DataType(DataType.MultilineText)]
        public string Questions { get; set; }
        public string LecturerID { get; set; }
        [StringLength(16)]
        public string Code { get; set; }

        public DateTime? SubmissionDate { get; set; }

        public virtual Lecturer Lecturer { get; set; }
        public virtual CourseBank Course { get; set; }
        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; }
    }
}
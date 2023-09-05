using DataAccessLayer.GeneralModel;

namespace DataAccessLayer.Models
{
    public class Assignment : BaseEntity
    {
        public string CourseID { get; set; }
        public string Questions { get; set; }
        public string LecturerID { get; set; }

        public virtual Lecturer Lecturer { get; set; }
        public virtual CourseBank Course { get; set; }
        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; }
    }
}
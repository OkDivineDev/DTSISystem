using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DataAccessLayer.GeneralModel;

namespace DataAccessLayer.Models
{
    public class CourseBank : BaseEntity
    {

        [Required(ErrorMessage = "*")]
        public string Code { get; set; }
        [Required(ErrorMessage = "*")]
        public string Title { get; set; }
        [Required(ErrorMessage = "*")]
        public int Unit { get; set; }
        [ForeignKey("Department")]
        public string DepartmentID { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<CourseAllocation>? CourseAllocations { get; set; }
        public virtual ICollection<Assignment>? Assignments { get; set; }
        public virtual ICollection<Chat>? Chats { get; set; }
    }
}
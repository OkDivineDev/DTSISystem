using DataAccessLayer.GeneralModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Lecturer : BaseEntity
    {
       
        public string UserId { get; set; }

        public string Name { get; set; }

        [NotMapped]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        


       public virtual ICollection<CourseAllocation>? CourseAllocations { set; get; }
       public virtual ICollection<Assignment>? Assignments { set; get; }
    }
}
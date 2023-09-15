using DataAccessLayer.GeneralModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class CourseOutLine : BaseEntity
    {
        [ForeignKey("Course")]
        public string CourseId { get; set; }

        [DataType(DataType.MultilineText)]
        public string OutLine { get; set; }

        public int SN { get; set; }

        public virtual CourseBank Course { get; set; }
    }
}
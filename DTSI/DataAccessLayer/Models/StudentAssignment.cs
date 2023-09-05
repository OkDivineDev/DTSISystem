using DataAccessLayer.GeneralModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class StudentAssignment : BaseEntity
    {
        public string StudentRegNo { get; set; }
        public string AssignmentID { get; set; }


        public virtual Student Student { get; set; }
        public virtual Assignment Assignment { get; set; }
    }
}

using DataAccessLayer.GeneralModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Admission : BaseEntity
    {
        public string Session { get; set; }
        public string RegNo { get; set; }
        public int EntryYear { get; set; }
        public int GraduationYear { get; set; }
        public string EntryMode { get; set; }
        public string JambRegNo { get; set; }
        public string Email { get; set; }
     
    }
}
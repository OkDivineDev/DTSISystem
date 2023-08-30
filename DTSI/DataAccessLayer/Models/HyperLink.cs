using DataAccessLayer.GeneralModel;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class HyperLink : BaseEntity
    {
        public string Link { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
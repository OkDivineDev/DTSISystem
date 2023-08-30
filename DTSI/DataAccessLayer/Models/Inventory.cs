

using DataAccessLayer.GeneralModel;

namespace DataAccessLayer.Models
{
    public class Inventory : BaseEntity
    {
        public string User { get; set; }
        public string Action { get; set; }
    }
}
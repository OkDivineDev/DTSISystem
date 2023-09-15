using DataAccessLayer.GeneralModel;

namespace DataAccessLayer.Models
{
    public class Chat : BaseEntity
    {
        public string UserID { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public string? Image { get; set; }

    }
}
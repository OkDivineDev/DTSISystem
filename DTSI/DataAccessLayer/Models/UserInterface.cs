using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class UserInterface
    {
        [Key]
        public string UserId { get; set; }
        public string Email { get; set; }




    
        
        public virtual Student? Student { get; set; }
        public virtual ICollection<Lecturer>? Lecturers { get; set; }


    }
}

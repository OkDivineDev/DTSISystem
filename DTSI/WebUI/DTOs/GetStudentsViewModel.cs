using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class GetStudentsViewModel
    {
        [Range(100,1000)]
        [Required(ErrorMessage ="Level must be selected")]
        public int Level { get; set; }
    }
}

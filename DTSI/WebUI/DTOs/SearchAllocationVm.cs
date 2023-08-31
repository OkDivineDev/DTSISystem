using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs
{
    public class SearchAllocationVm
    {
        [Display(Name = "Academic Session")]
        public string? _Session { get; set; }
    }
}

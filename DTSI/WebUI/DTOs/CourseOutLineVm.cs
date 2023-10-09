using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebUI.DTOs
{
    public class CourseOutLineVm
    {
        
        public string? Id { get; set; }
        public string? CourseId { get; set; }
        
        [DataType(DataType.MultilineText)]
        [RegularExpression(@"^[\w\-'’“”;:&`(),.0-9\s]+$",
         ErrorMessage = "Not in proper format!")]
        [Display(Name = "Course Outline")]
        public string? OutLine { get; set; }

        public int? SN { get; set; }



    }
}
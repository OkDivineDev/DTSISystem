// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace WebUI.Areas.Identity.Pages.Account.Manage
{
    public class StudentsProfileModel : PageModel
    {
        private readonly IRepository<Student> repoStd;
        private readonly IRepository<UserInterface> repoUser;

        public StudentsProfileModel(IRepository<Student> _repoStd, IRepository<UserInterface> _repoUser)
        {
            repoStd = _repoStd;
            repoUser = _repoUser;
        }

        [BindProperty]
        public InputModel Input { get; set; }


        [TempData]
        public string StatusMessage { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {

            [Required]
            [Display(Name = "Matric Number")]
            public string RegNo { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Entry Year")]
            public int EntryYear { get; set; }

            [Required]
            [Display(Name = "Graduation Year")]
            public int GraduationYear
            {
                get; set;
            }
            [Required]
            [Display(Name = "Entry Mode")]
            public string EntryMode
            {
                get; set;
            }
            [Required]
            [Display(Name = "Jamb Reg Number")]
            public string JambRegNo { get; set; }

            [Required]
            public string Gender { get; set; }

            [Required]
            public int Level { get; set; }

            [Required]
            public string Passport { get; set; }


        }

        public async Task<IActionResult> OnGet()
        {
            var userName = User.Identity.Name;

            InputModel input = new();

            var userApp = await repoUser.GetByIdAsync(x => x.Email == userName);
            if (userApp != null)
            {
                var std = await repoStd.GetByIdAsync(st => st.UserId == userApp.UserId);
                if (std != null)
                {
                    input = new InputModel()
                    {
                        EntryMode = std.EntryMode,
                        Gender = std.Gender,
                        JambRegNo = std.JambRegNo,
                        EntryYear = std.EntryYear,
                        GraduationYear = std.GraduationYear,
                        Level = std.Level,
                        RegNo = std.RegNo,
                        Name = std.Name,
                        Passport = std.Passport
                    };
                }
                else
                {
                    StatusMessage = "NO STUDENT RECORD FOUND";
                }
            }
            else
            {
                StatusMessage = "SOMETHING WENT WRONG, TRY AGAIN";
            }



            Input = input;

            return Page();


        }
    }
}

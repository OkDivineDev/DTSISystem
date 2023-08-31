// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces;
using DataAccessLayer.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using WebUI.DTOs;

namespace WebUI.Areas.Identity.Pages.Account
{
    public class StudentRegisterModel : PageModel
    {
        private string v = "";
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IRepository<UserInterface> _repoUser;
        private readonly IRepository<Admission> _repoAdmission;
        private readonly IRepository<Student> _repoStd;
        private readonly IEmailSender _emailSender;
        readonly StudentRegisterHelper studentRegisterHelper = new();
        private readonly LocalInfrastructure local = new();

        public StudentRegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IRepository<UserInterface> repoUser,
            IRepository<Admission> repoAdmission,
            IRepository<Student> repoStd,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _repoUser = repoUser;
            _repoAdmission = repoAdmission;
            _repoStd = repoStd;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public StudentInputModel StudentInput { get; set; }



        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class StudentInputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }



            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string DepartmentID { get; set; }

            [Required]
            public string RegNo { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public int EntryYear { get; set; }
            [Required]
            public int GraduationYear { get; set; }
            [Required]
            public string EntryMode { get; set; }
            [Required(ErrorMessage = "Jamb Reg No is required!")]
            public string JambRegNo { get; set; }
            [Required(ErrorMessage = "Gender is required!")]
            public string Gender { get; set; }
            [Required(ErrorMessage = "Level is required!")]
            public int Level { get; set; }

            public string? Passport { get; set; }
            public IFormFile PassportFile { get; set; }

        }


        public async Task<IActionResult> OnGetAsync(string regNo, string email, string code)
        {


            if (regNo != null && code != null && email != null)
            {
                string regno = regNo;
                var stdAd = await _repoAdmission.GetByIdAsync(x => x.RegNo == regno);
                if (stdAd != null)
                {
                    //RETURN STUDENT RECORD THROUGH VIEW MODEL
                    var dbStd = new StudentInputModel()
                    {
                        RegNo = stdAd.RegNo,
                        JambRegNo = stdAd.JambRegNo,
                        EntryMode = stdAd.EntryMode,
                        EntryYear = stdAd.EntryYear,
                        Email = stdAd.Email,
                        DepartmentID = stdAd.DepartmentId,
                        GraduationYear = stdAd.GraduationYear,
                    };

                    StudentInput = dbStd;
                    TempData["stdRecord"] = dbStd;

                    TempData["levels"] = studentRegisterHelper.GetLevels();
                    TempData["genders"] = studentRegisterHelper.GetGender();

                    return Page();
                }

                //TempData[v] = "Hi, your record was not found in any admission list!";
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                if (StudentInput.PassportFile == null)
                {
                    var error = "Please select your passport image!";
                    ModelState.AddModelError(string.Empty, error);
                }

                else if (StudentInput.PassportFile.Length > 100000)
                {
                    var error = "Please reduce the size of your passport to not more than 100KB!";
                    ModelState.AddModelError(string.Empty, error);
                }
                else if (StudentInput.PassportFile.FileName.ToLower().EndsWith("jpg") ||
                         StudentInput.PassportFile.FileName.ToLower().EndsWith("jpeg") ||
                         StudentInput.PassportFile.FileName.ToLower().EndsWith("png"))
                {

                    //UPLOAD NOT EMPTY
                    string passName = StudentInput.RegNo + Path.GetExtension(StudentInput.PassportFile.FileName);


                    string path = Path.Combine("wwwroot", "Media", "StudentPassport", StudentInput.PassportFile.FileName.Replace(" ", "_"));

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    await local.LocalImageStore(path, StudentInput.PassportFile);

                    StudentInput.Passport = passName;
                }
                else
                {
                    var error = "Please convert your image file to png, jpg or jpeg!";
                    ModelState.AddModelError(string.Empty, error);
                }


                //  CREATE USER ACCOUNT

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, StudentInput.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, StudentInput.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, StudentInput.Password);

                if (result.Succeeded)
                {

                    //CREATE USERINTERFACE
                    _repoUser.Add(new UserInterface()
                    {
                        Email = user.Email,
                        UserId = user.Id
                    });

                    //  CREATE STUDENT ACCOUNT

                    var newStd = new Student()
                    {
                        UserId = user.Id,
                        DepartmentID = StudentInput.DepartmentID,
                        RegNo = StudentInput.RegNo,
                        JambRegNo = StudentInput.JambRegNo,
                        EntryMode = StudentInput.EntryMode,
                        EntryYear = StudentInput.EntryYear,
                        GraduationYear = StudentInput.GraduationYear,
                        Gender = StudentInput.Gender,
                        Level = StudentInput.Level,
                        Passport = StudentInput.Passport,
                        Name = StudentInput.Name
                    };

                    if (newStd != null)
                    {
                        if (_repoStd.Add(newStd))
                        {

                            _logger.LogInformation("User created a new account with password.");

                            var userId = await _userManager.GetUserIdAsync(user);
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                            var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                                protocol: Request.Scheme);

                            await _emailSender.SendEmailAsync(StudentInput.Email, "Confirm your email",
                                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                            if (_userManager.Options.SignIn.RequireConfirmedAccount)
                            {
                                return RedirectToPage("RegisterConfirmation", new { email = StudentInput.Email, returnUrl = returnUrl });
                            }
                            else
                            {
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);
                            }
                        }
                        else
                        {
                            //  DELETE STUDENT USER ACCOUNT FROM 
                            //  1. USER_INTERFACE
                            //  2. USER
                            var userInterface = await _repoUser.GetByIdAsync(u => u.Email == user.Email);
                            if (userInterface != null)
                                _repoUser.Delete(userInterface);
                            await _userManager.DeleteAsync(user);
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }


            var stdAd = await _repoAdmission.GetByIdAsync(x => x.RegNo == StudentInput.RegNo);

            var dbStd = new StudentInputModel()
            {
                RegNo = stdAd.RegNo,
                JambRegNo = stdAd.JambRegNo,
                EntryMode = stdAd.EntryMode,
                EntryYear = stdAd.EntryYear,
                Email = stdAd.Email,
                DepartmentID = stdAd.DepartmentId,
                GraduationYear = stdAd.GraduationYear,
            };



            StudentInput = dbStd;

            TempData["levels"] = studentRegisterHelper.GetLevels();
            TempData["genders"] = studentRegisterHelper.GetGender();






            // If we got this far, something failed, redisplay form
            return Page();
        }



        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}

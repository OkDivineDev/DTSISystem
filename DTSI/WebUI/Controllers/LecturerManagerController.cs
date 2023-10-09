using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces;
using ClosedXML.Excel;
using DataAccessLayer.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using WebUI.DTOs;

namespace WebUI.Controllers
{
    public class LecturerManagerController : Controller
    {
        private readonly LocalInfrastructure local = new();
        private readonly INotyfService notyfService;
        private readonly IUserStore<IdentityUser> userStore;
        private readonly IHttpContextAccessor context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IRepository<Inventory> repoInventory;
        private readonly IRepository<HyperLink> repoHyLink;
        private readonly IRepository<UserInterface> repoUserInterface;
        private readonly IRepository<Department> repoDept;
        private readonly IRepository<Lecturer> repoLec;
        private readonly PopNotification popNotification;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly string v = "Msg";

        public LecturerManagerController(INotyfService _notyfService,
                                         IUserStore<IdentityUser> userStore,
                                         IHttpContextAccessor _context,
                                         UserManager<IdentityUser> userManager,
                                         IRepository<Inventory> _repoInventory,
                                         IRepository<HyperLink> _repoHyLink,
                                         IRepository<UserInterface> _repoUserInterface,
                                         IRepository<Department> _repoDept,
                                         IRepository<Lecturer> _repoLec
                                         )
        {
            notyfService = _notyfService;
            this.userStore = userStore;
            context = _context;
            this.userManager = userManager;
            repoInventory = _repoInventory;
            repoHyLink = _repoHyLink;
            repoUserInterface = _repoUserInterface;
            repoDept = _repoDept;
            repoLec = _repoLec;
            _emailStore = GetEmailStore();
            popNotification = new PopNotification(notyfService);
        }

        // GET: Employee
        [Authorize(Roles = "HOD,DepartmentICT,Lecturer")]
        public async Task<IActionResult> Index()
        {
            string msg = null;
            //GET ALL EMPLOYEE AS ADMIN
            try
            {
                var users = await repoUserInterface.GetAll();
                IEnumerable<LecturerVm> lecturers = null;
                string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
                string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;


                bool isHod = await IsHod(sessionEmpId, sessionDeptId);
                ViewBag.IsHod = isHod;

                if (User.IsInRole("DepartmentICT"))
                {
                    var user = users.FirstOrDefault(x => x.Email == User.Identity.Name);
                    var userLec = await repoLec.GetByIdAsync(x => x.UserId == user.UserId);

                    var allLecturer = await repoLec.GetByQueryAsync(x => x.DepartmentID == userLec.DepartmentID);
                    _ = allLecturer.OrderBy(x => x.Name);

                    if (allLecturer.Any())
                        lecturers = await PopulateLecturer(allLecturer, false);



                    ViewBag.Lecturers = lecturers.ToList();
                    ViewBag.Count = lecturers.Count();
                }
                else if (User.IsInRole("HOD"))
                {
                    var user = users.FirstOrDefault(x => x.Email == User.Identity.Name);
                    //var userEmp = await repoEmp.GetByIdAsync(x => x.UserId == user.UserId);
                    var userDept = await repoDept.GetByIdAsync(x => x.HODUserId == user.UserId);

                    if (userDept != null)
                    {
                        var allLecturer = await repoLec.GetByQueryAsync(x => x.DepartmentID == userDept.Id);
                        _ = allLecturer.OrderBy(x => x.Name);

                        if (allLecturer.Any())
                            lecturers = await PopulateLecturer(allLecturer, true);
                    }
                    else
                    {
                        var allLecturer = await repoLec.GetByQueryAsync(x => x.UserId == user.UserId);

                        lecturers = await PopulateLecturer(allLecturer, false);
                    }

                    ViewBag.Lecturers = lecturers.ToList();
                    ViewBag.Count = lecturers.Count();
                }
                else
                {
                    //GET EMPLOYEE IN THE HOD'S DEPARTMENT
                    string userId = null;
                    var user = users.FirstOrDefault(x => x.Email == User.Identity.Name);
                    if (user != null)
                    {
                        userId = user.UserId;
                    }
                    else
                    {
                        TempData[v] = "Error, User not found!";
                        return RedirectToAction("Index", "AdminManager");
                    }

                    var emps = await repoLec.GetByQueryAsync(x => x.UserId == userId);

                    if (emps.Any())
                    {
                        lecturers = await PopulateLecturer(emps, false);


                        ViewBag.Lecturers = lecturers.ToList();
                        ViewBag.Count = lecturers.Count();
                    }
                    else
                    {
                        TempData[v] = "Error, Your employee Record not found!";
                        return RedirectToAction("Index", "AdminManager");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!";

                return RedirectToAction("Index", "AdminManager");
            }

            if (TempData[v] != null)
            {
                popNotification.Notyf(TempData[v].ToString());
            }

            return View();
        }

        #region EDIT EMPLOYEE RECORD

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HOD,DepartmentICT")]
        public async Task<JsonResult> Index(LecturerVm model)
        {
            string msg = "";

            if (ModelState.IsValid)
            {
                try
                {
                    var dbEmp = await repoLec.GetByIdAsync(o => o.UserId == model.UserId);
                    if (dbEmp != null)
                    {
                        dbEmp.Name = model.Name;
                        repoLec.Update(dbEmp);
                        msg = "Operation completed successfully!";
                    }
                }
                catch (Exception ex)
                {
                    msg = "Fatal Error, please try again or contact admin!";
                }
            }
            else
            {
                string error = "";
                foreach (var err in ModelState)
                {
                    if (err.Value.Errors.Any())
                        error += "[" + err.Key + "]: " + err.Value.Errors.FirstOrDefault().ErrorMessage;
                }

                msg = error;
            }

            TempData[v] = msg;
            return Json(msg);
        }

        [Authorize(Roles = "HOD,DepartmentICT")]
        public async Task<IActionResult> EditLecturer(string ID)
        {
            try
            {
                LecturerVm model = new();
                if (!string.IsNullOrEmpty(ID))
                {
                    var obj = await repoLec.GetByIdAsync(o => o.Id == ID);
                    if (obj != null)
                    {
                        model.Id = obj.Id;
                        model.Name = obj.Name;
                        model.UserId = obj.UserId;
                    }
                    return PartialView("EditLecturer", model);
                }
            }
            catch (Exception ex)
            {
                popNotification.Notyf("Fatal Error, please contact admin!");
            }
            return PartialView("EditLecturer");
        }

        #endregion EDIT EMPLOYEE RECORD

        #region ADMIN UPLOAD EMPLOYEE RECORDS

        [Authorize(Roles = "HOD,DepartmentICT")]
        public IActionResult UploadLecturer()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "HOD,DepartmentICT")]
        public async Task<ActionResult> UploadLecturer(ExcelUploadFile model)
        {
            string msg = "";
            int countDone = 0;
            int countNotDone = 0;

            ErrorMessageViewModel errorMessage = new();
            try
            {
                string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
                string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;

                if (ModelState.IsValid && sessionDeptId != null)
                {
                    //CHECK IF UPLOAD FILE IS EMPTY
                    if (model.ExcelFile.Name == null || model.ExcelFile.Length < 1)
                    {
                        msg = "Please select excel file and Try again!";
                        popNotification.Notyf(msg);
                        return View(); ;
                    }
                    else if (model != null && model.ExcelFile.Length > 0 && model.ExcelFile.FileName.EndsWith("xls") || model.ExcelFile.FileName.EndsWith("xlsx"))
                    {
                        //GET INSTITUTION ID FROM INSTITUTION ITF ADMIN UPLOADING
                        string deptID = "";
                        string userId = null;
                        bool isSaved = false;

                        var user = await repoUserInterface.GetByIdAsync(x => x.Email == User.Identity.Name);
                        userId = user.UserId;

                        //var emp = await repoEmp.GetByIdAsync(x => x.UserId == userId);
                        if (sessionEmpId == null)
                        {
                            popNotification.Notyf("Error, Your Employee Record was not found!");
                            return RedirectToAction("Index", "Home");
                        }

                        deptID = sessionDeptId;

                        //UPLOAD NOT EMPTY

                        //UPLOAD NOT EMPTY Uploads/Admission/
                        string path = Path.Combine("wwwroot", "TempUpload", model.ExcelFile.FileName.Replace(" ", "_"));

                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);

                        await local.LocalImageStore(path, model.ExcelFile);

                        //STUDENT EMAIL COLLECTIONS

                        var uploadResponses = new List<UploadResponse>();
                        List<EmailViewModel> EmailsBox = new List<EmailViewModel>();

                        XLWorkbook xLWorkbook = new(path);

                        int startRow = 3;
                        while (xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 1).GetString() != "")
                        {
                            Lecturer newLec = new(); //EMPLOYEE HOLDER

                            UploadResponse upRes = new();
                            string ms = "";

                            try
                            {
                                newLec.DepartmentID = deptID;

                                newLec.Name = xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 2).GetString();
                                newLec.Email = xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 3).GetString();

                                if (!IsEmailValid(newLec.Email))
                                {
                                    ms = newLec.Name + ": Unable to create user Account and employee record due to invalid email address: " + newLec.Email;
                                    //ADD RESPONSE TO UPLOAD RESPONSE
                                    upRes.Message = ms;
                                    uploadResponses.Add(upRes);
                                    countNotDone += 1;
                                }
                                else
                                {
                                    var oldUser = await repoUserInterface.GetByIdAsync(x => x.Email == newLec.Email);
                                    if (oldUser != null)
                                    {
                                        var oldSameEmp = await repoLec.GetByIdAsync(x => x.UserId == oldUser.UserId && x.DepartmentID == sessionDeptId);
                                        if (oldSameEmp == null)
                                        {
                                            newLec.UserId = oldUser.UserId;
                                            isSaved = repoLec.Add(newLec);

                                            if (isSaved)
                                            {
                                                string m = string.Empty;

                                                var _user = new IdentityUser()
                                                {
                                                    Id = oldUser.UserId,
                                                    Email = oldUser.Email
                                                };
                                                
                                                //  ADD TO LECTURER ROLE
                                                var lectRole_result = await userManager.AddToRoleAsync(_user, "Lecturer");
                                                if (lectRole_result.Succeeded)
                                                {
                                                    m = " added to Lecturer Role";
                                                }

                                                ms = $"{newLec.Name} lecturer record was created and {m} succesfully!";
                                                countDone += 1;
                                            }
                                            else
                                            {
                                                ms = $"{newLec.Name} lecturer record was not created!";
                                                countNotDone += 1;
                                            }
                                        }
                                        else
                                        {
                                            ms = $"{newLec.Name} has already been added on the same department!";
                                            countNotDone += 1;
                                        }
                                        //ADD RESPONSE TO UPLOAD RESPONSE
                                        upRes.Message = ms;
                                        uploadResponses.Add(upRes);
                                    }
                                    else
                                    {
                                        string password = newLec.Email.Substring(0, 5).ToLower() + "@Z12345";

                                        if (newLec.Email != null && newLec.Name != null)
                                        {
                                            var empEmail = new EmailViewModel()
                                            {
                                                Email = newLec.Email,
                                                RegNo = password
                                            };
                                            EmailsBox.Add(empEmail);
                                        }

                                        //CREATE USER ACCOUNT

                                        var newUser = CreateUser();
                                        await userStore.SetUserNameAsync(newUser, newLec.Email, CancellationToken.None);
                                        await _emailStore.SetEmailAsync(newUser, newLec.Email, CancellationToken.None);
                                        var result = await userManager.CreateAsync(newUser, password);

                                        if (result.Succeeded)
                                        {
                                            //CREATE USERINTERFACE
                                            repoUserInterface.Add(new UserInterface()
                                            {
                                                Email = newUser.Email,
                                                UserId = newUser.Id
                                            });

                                            newLec.UserId = newUser.Id;
                                            isSaved = repoLec.Add(newLec);

                                            if (isSaved)
                                            {
                                                string m = string.Empty;

                                                var lectRole_result = await userManager.AddToRoleAsync(newUser, "Lecturer");
                                                if (lectRole_result.Succeeded)
                                                {
                                                    m = " added to Lecturer Role";
                                                }

                                                ms = $"{newLec.Name} user account and lecturer record was created and lecturer is {m} succesfully!";
                                                countDone += 1;
                                            }
                                            else
                                            {
                                                ms = $"{newLec.Name} user account was created but lecturer record was not created!";
                                                countNotDone += 1;
                                            }
                                            //ADD RESPONSE TO UPLOAD RESPONSE
                                            upRes.Message = ms;
                                            uploadResponses.Add(upRes);
                                        }
                                        else
                                        {
                                            ms = newLec.Name + ": Unable to create user Account, hence lecturer record failed " + result.Errors.FirstOrDefault().ToString();
                                            //ADD RESPONSE TO UPLOAD RESPONSE
                                            upRes.Message = ms;
                                            uploadResponses.Add(upRes);
                                            countNotDone += 1;
                                        }
                                        foreach (var error in result.Errors)
                                        {
                                            ModelState.AddModelError(string.Empty, error.Description);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                string _msg = newLec.Name;

                                _msg += " record encountered Fatal Error while processing, please contact admin!";

                                //ADD RESPONSE TO UPLOAD RESPONSE
                                upRes.Message = _msg;
                                uploadResponses.Add(upRes);
                                countNotDone += 1;
                            }

                            startRow++;
                        }

                        var inventory = new Inventory()
                        {
                            User = User.Identity.Name,
                            Action = "The user uploaded new employee records to the department."
                        };

                        repoInventory.Add(inventory);

                        //DELETE THE FILE
                        System.IO.File.Delete(path);

                        //EMPLOYEE AND USER ACCOUNT WAS SUCCESSFULLY SAVED

                        if (isSaved) // GREATER THAN 1 MEANS SAVED
                        {
                            string mail = "";

                            //SEND MAIL TO EACH ADMITTED STUDENT
                            foreach (var empEmail in EmailsBox)
                            {
                                //IdentityMessage ITidMesStd = new IdentityMessage()
                                //{
                                //    Destination = empEmail.Email,
                                //    Subject = "Account Registration Link"
                                //};

                                //string code = Guid.NewGuid().ToString() + DateTime.Now.ToString().Trim();
                                //var callbackUrl = Url.Action("ResetPassword", "Account", new { code = "permissiontochangedefaultpassword" }, protocol: Request.Url.Scheme);

                                //string message = "Your default password is: " + empEmail.RegNo + ", Please change the default password by clicking <a href=\"" + callbackUrl + "\">here</a>";

                                //MailHelper sendMail = new MailHelper();
                                //ConfirmEmailSend sendMsg = sendMail.SendMail(ITidMesStd, message);

                                ////ADD LINK TO HYPERLINKS
                                //var link1 = new HyperLink()
                                //{
                                //    Link = callbackUrl + empEmail.RegNo,
                                //    Email = empEmail.Email,
                                //};
                                //repoHyLink.Add(link1);
                            }

                            //INFORM THE UPLOADER OF THE UPLOAD MADE
                            //var _user = User.Identity.Name;

                            //IdentityMessage ITidMes = new IdentityMessage()
                            //{
                            //    Destination = _user,
                            //    Subject = "Upload response"
                            //};

                            ////GO THROUGH ALL THE RESPONSES AND SEND MAIL TO INSTITUTION ITF EMAIL
                            //foreach (var m in uploadResponses)
                            //{
                            //    mail += "<hr />" + m.Message;
                            //}

                            //MailHelper sendMail1 = new MailHelper();
                            //ConfirmEmailSend sendMsg1 = sendMail1.SendMail(ITidMes, mail);

                            //ADD LINK TO HYPERLINKS
                            //var link = new HyperLink()
                            //{
                            //    Link = mail,
                            //    Email = _user,
                            //};

                            //repoHyLink.Add(link);

                            msg = "(" + countDone.ToString() + ") employee(s) uploaded successfully and (" + countNotDone + ") were not, please check your email for more details!";
                        }
                        else
                        {
                            string mail = "";
                            foreach (var m in uploadResponses)
                            {
                                mail += "\n" + m.Message;
                            }
                            msg = mail;
                        }
                    }
                    else
                    {
                        msg = "Please select a valid file, and Try again!";
                        popNotification.Notyf(msg);

                        return View();
                    }
                }
                else
                {
                    string error = "";
                    foreach (var err in ModelState)
                    {
                        if (err.Value.Errors.Any())
                            error += "[" + err.Key + "]: " + err.Value.Errors.FirstOrDefault().ErrorMessage;
                        else
                            error = "Logout, login with your department selected and try again!";
                    }

                    msg = error;
                }
            }
            catch (Exception)
            {
                msg = "Fatal Error, please try again or contact the admin!";
            }


            if (!string.IsNullOrEmpty(msg))
                popNotification.Notyf(msg);
            return View();
        }

        [Authorize(Roles = "HOD")]
        public async Task<IActionResult> DeleteLecturer(string ID)
        {
            string result = "";
            if (!string.IsNullOrEmpty(ID))
            {
                var objDel = await repoLec.GetByIdAsync(o => o.Id == ID);
                if (objDel != null)
                {
                    if (repoLec.Delete(objDel))
                        result = $"{objDel.Name} employee record was successfully deleted!";
                    else
                        result = "Error, operation failed, try again or contact admin!";

                    var inventory = new Inventory()
                    {
                        User = User.Identity.Name,
                        Action = $"The user tried to delete {objDel.Name} employee record from the department."
                    };
                    repoInventory.Add(inventory);
                }
                else
                {
                    result = "Failed!";
                }
            }
            else
            {
                result = "Failed!";
            }

            if (result != "")
            {
                popNotification.Notyf(result);
            }

            return RedirectToAction("Index");
        }

        #endregion ADMIN UPLOAD EMPLOYEE RECORDS

        private async Task<IEnumerable<LecturerVm>> PopulateLecturer(IEnumerable<Lecturer> dbLecturers, bool isHod)
        {
            var users = await repoUserInterface.GetAll();
            var employees = new List<LecturerVm>();

            foreach (var em in dbLecturers)
            {
                var dept = await repoDept.GetByIdAsync(x => x.Id == em.DepartmentID);
                employees.Add(
                new LecturerVm()
                {
                    DepartmentID = dept.Code,
                    Name = em.Name,
                    Id = em.Id,
                    Email = users.FirstOrDefault(c => c.UserId == em.UserId).Email,
                    UserId = em.UserId
                });
            }

            if (isHod)
            {
                var user = users.FirstOrDefault(x => x.Email == User.Identity.Name);

                //GET HOD IF BELONG TO ANOTHER DEPARTMENT AS LECTURER
                var hodEmployees = await repoLec.GetByQueryAsync(x => x.UserId == user.UserId);
                if (hodEmployees.Any())
                    foreach (var em in hodEmployees)
                    {
                        var exist = employees.FirstOrDefault(x => x.UserId == em.UserId);
                        if (exist == null)
                        {
                            var dept = await repoDept.GetByIdAsync(x => x.Id == em.DepartmentID);
                            employees.Add(
                            new LecturerVm()
                            {
                                DepartmentID = dept.Code,
                                Name = em.Name,
                                Email = users.FirstOrDefault(c => c.UserId == em.UserId).Email,
                                UserId = em.UserId
                            });
                        }
                    }
            }
            return employees.ToList();
        }

        private bool IsEmailValid(string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        private async Task<bool> IsHod(string? empId, string? deptId)
        {
            var isHodToDept = await repoLec.GetByIdAsync(x => x.Id == empId);
            var dept = await repoDept.GetByIdAsync(x => x.Id == deptId && x.HODUserId == isHodToDept.UserId);

            if (dept != null)
                return true;
            else
                return false;
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
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)userStore;
        }
    }
}
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces;
using ClosedXML.Excel;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Security.Principal;
using WebUI.Data;
using WebUI.DTOs;

namespace WebUI.Controllers
{
    public class DepartmentController : Controller
    {
        private const string v = "Msg";
        private readonly LocalInfrastructure local = new();

    
        private readonly IRepository<Department> repoDept;
        private readonly IHttpContextAccessor context;
        private readonly IRepository<Student> repoStd;
        private readonly IRepository<AcademicSession> repoAcaSession;
        private readonly IRepository<Lecturer> repoLec;
        private readonly IRepository<Inventory> repoInventory;
        private readonly INotyfService notyfService;
        private readonly IRepository<HyperLink> repoHyperlink;
        private readonly IRepository<Admission> repoAdmission;
        private readonly IRepository<UserInterface> repoUserInterface;

        private readonly PopNotification popNotification;

        public DepartmentController(
                                        IRepository<Inventory> _repoInventory,
                                        INotyfService _notyfService,
                                        IRepository<Department> _repoDept,
                                        IHttpContextAccessor _context,
                                        IRepository<Student> _repoStd,
                                        IRepository<AcademicSession> _repoAcaSession,
                                        IRepository<Lecturer> _repoLec,
                                        IRepository<HyperLink> _repoHyperlink,
                                        IRepository<Admission> _repoAdmission,
                                        IRepository<UserInterface> _repoUserInterface)
        {
            repoDept = _repoDept;
            context = _context;
            repoStd = _repoStd;
            repoAcaSession = _repoAcaSession;
            repoLec = _repoLec;
            repoInventory = _repoInventory;
            notyfService = _notyfService;
            repoHyperlink = _repoHyperlink;
            repoAdmission = _repoAdmission;
            repoUserInterface = _repoUserInterface;

            popNotification = new PopNotification(notyfService);
        }

        // GET: Department

        #region DEPARTMENT CRUD

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var deptsModel = new List<DepartmentViewModel>();

                // List<Department> departments = new List<Department>();

                //string userId = string.Empty;

                if (User.IsInRole("SchoolOwner") || User.IsInRole("Admin"))
                {
                    var departments = await repoDept.GetAll();
                    ViewBag.Count = departments.Count();

                    foreach (var dept in departments)
                    {
                        var emp = await repoLec.GetByIdAsync(x => x.UserId == dept.HODUserId);

                        deptsModel.Add(
                             new DepartmentViewModel()
                             {
                                 Name = dept.Name,
                                 Code = dept.Code,
                                 HODEmail = dept.HODEmail,
                                 HODName = emp.Name,
                                 Id = dept.Id
                             });
                    }

                    ViewBag.DepartmentList = deptsModel;
                }
                else
                {
                    var user = await repoUserInterface.GetByIdAsync(x => x.Email == User.Identity.Name);

                    var emp = await repoLec.GetByIdAsync(x => x.UserId == user.UserId);
                    if (emp != null)
                    {
                        var department = await repoDept.GetByIdAsync(x => x.Id == emp.DepartmentID);
                        if (department != null)
                        {
                            deptsModel.Add(new DepartmentViewModel()
                            {
                                Name = department.Name,
                                Code = department.Code,
                                HODEmail = department.HODEmail,
                                HODName = emp.Name,
                                Id = department.Id
                            });

                            ViewBag.Count = deptsModel.Count;

                            ViewBag.DepartmentList = deptsModel;
                        }
                        else
                        {
                            TempData[v] = "Hi, your school has no department at the moment, " +
                                "please kindly add department(s)!";
                        }
                    }
                    else
                    {
                        TempData[v] = "Oops, I could not find find your employee record!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!";
            }

            if (TempData[v] != null)
            {
                popNotification.Notyf(TempData[v].ToString());
            }
            return View();
        }

        //SAVE AND UPDATE ALL SCHOOL
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Index(DepartmentViewModel model)
        {
            string msg = "";

            if (ModelState.IsValid && model.HODEmail != null)
            {
                try
                {
                    //GET ADMINID FOR USER
                    var adminUser = await repoUserInterface.GetByIdAsync(x => x.Email == model.HODEmail);
                    if (adminUser == null)
                    {
                        msg = "Please the provided HOD Email address was not found on " +
                            "the database, HOD must be a registered user of " +
                            "our system!";
                    }
                    else
                    {
                        var dbObj = await repoDept.GetByIdAsync(o => o.Id == model.Id);

                        if (model.Id != null && dbObj != null)
                        {
                            dbObj.Name = model.Name;
                            dbObj.Code = model.Code;
                            dbObj.HODUserId = adminUser.UserId;
                            if (repoDept.Update(dbObj))
                            {
                                //  CHECK IF EMPLOYEE RECORD EXIST FOR THE HOD
                                var emp = await repoLec.GetByIdAsync(id => id.UserId == dbObj.HODUserId);
                                // ELSE ADD
                                if (emp == null)
                                {
                                    var HodEmp = new Lecturer()
                                    {
                                        UserId = dbObj.HODUserId,
                                        DepartmentID = dbObj.Id,
                                        Name = model.HODName
                                    };

                                    repoLec.Add(HodEmp);
                                }
                                msg = "Department Updated successfully!";
                            }
                        }
                        else
                        {
                            
                            Department objDepart = new()
                            {
                                Name = model.Name,
                                Code = model.Code,
                                HODUserId = adminUser.UserId
                            };

                            if (repoDept.Add(objDepart))
                            {
                                // CREATE EMPLOYEE RECORD FOR THE HOD
                                var emp = new Lecturer()
                                {
                                    UserId = objDepart.HODUserId,
                                    DepartmentID = objDepart.Id,
                                    Name = model.HODName
                                };
                                if (repoLec.Add(emp))
                                {
                                    msg = "Department created and HOD record added to " +
                                        "employee table, process completed successfully!";
                                }
                                else
                                    msg = "Department created successfully but HOD was not added " +
                                        "to employee record, please upload HOD record to Employee!";
                                var inventory = new Inventory()
                                {
                                    User = User.Identity.Name,
                                    Action = "The user added/modified " + model.Name
                                };
                                repoInventory.Add(inventory);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = "SORRY, TRY AGAIN, IF PERSISTED, CONTACT ADMIN!";
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

        //ADD AND EDIT SCHOOL
        [Authorize(Roles = "Admin,SchoolOwner")]
        public async Task<IActionResult> AddEditDepartment(string ID)
        {
            try
            {
                DepartmentViewModel model = new();
                if (!string.IsNullOrEmpty(ID))
                {
                    var obj = await repoDept.GetByIdAsync(o => o.Id == ID);
                    if (obj != null)
                    {
                        var emp = await repoLec.GetByIdAsync(x => x.UserId == obj.HODUserId);
                        var hodUsser = await repoUserInterface.GetByIdAsync(x => x.UserId == emp.UserId);

                        model.Id = obj.Id;
                        model.Name = obj.Name;
                        model.Code = obj.Code;
                        model.HODName = emp.Name;
                        model.HODEmail = hodUsser.Email;
                    }
                }

                

                return PartialView("AddEditDepartment", model);
            }
            catch (Exception ex)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!\n";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin,SchoolOwner")]
        public async Task<IActionResult> DeleteDepartment(string ID)
        {
            if (ID != null)
            {
                var objDel = await repoDept.GetByIdAsync(o => o.Id == ID);
                if (objDel != null)
                {

                    repoDept.Delete(objDel);

                    var inventory = new Inventory()
                    {
                        User = User.Identity.Name,
                        Action = "The user deleted " + objDel.Name
                    };
                    repoInventory.Add(inventory);

                    TempData[v] = "Department and all associated record " +
                        "has been deleted successfully!";
                }
                else
                {
                    TempData[v] = "Record may've been already deleted!";
                }
            }
            else
            {
                TempData[v] = "Record was not deleted!";
            }
            return RedirectToAction("Index");
        }

      
        #endregion DEPARTMENT CRUD

        #region ADMIN UPLOAD STUDENT REGISTRATION NUMBER YEARLY AFTER ADMISSION

        [HttpGet]
        [Authorize(Roles = "DepartmentICT,HOD")]
        public ActionResult UploadStudent()
        {
            if (TempData[v] != null)
            {
                string response = TempData[v].ToString();
                popNotification.Notyf(response);

            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "DepartmentICT,HOD")]
        public async Task<IActionResult> UploadStudent(ExcelUploadFile model)
        {
            string msg = "";
            int countDone = 0;
            int countNotDone = 0;

            string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
            string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;


            try
            {
                if (ModelState.IsValid && sessionDeptId != null && sessionEmpId != null)
                {
                    //CHECK IF UPLOAD FILE IS EMPTY
                    if (model == null)
                    {
                        msg = "Please select excel file and Try again!";

                        return RedirectToAction("UploadStudent");
                    }
                    else if (model != null && model.ExcelFile.FileName.ToLower().EndsWith("xls") || model.ExcelFile.FileName.ToLower().EndsWith("xlsx"))
                    {
                        string deptID = "";

                        deptID = sessionDeptId;


                        //UPLOAD NOT EMPTY Uploads/Admission/
                        string path = Path.Combine("wwwroot", "TempUpload", "Admission", model.ExcelFile.FileName.Replace(" ", "_"));

                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);

                        await local.LocalImageStore(path, model.ExcelFile);

                        //STUDENT EMAIL COLLECTIONS

                        var uploadResponses = new List<UploadResponse>();
                        List<EmailViewModel> StudentEmailsBox = new();

                        //ITERATE THROUGH ALL AVAILABLE STUDENT ON THE LIST

                        XLWorkbook xLWorkbook = new(path);

                        int startRow = 3;
                        while (xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 1).GetString() != "")
                        {
                            Admission admission = new(); //STUDENT HOLDER

                            UploadResponse upRes = new(); // RESPONSE TO ITF ADMIN

                            admission.DepartmentId = deptID;

                            admission.Session = xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 2).GetString();
                            admission.RegNo = xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 3).GetString();
                            admission.EntryYear = Convert.ToInt32(xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 4).GetString());
                            admission.GraduationYear = Convert.ToInt32(xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 5).GetString());
                            admission.EntryMode = xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 6).GetString();
                            admission.JambRegNo = xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 7).GetString();
                            admission.Email = xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 8).GetString();

                            if (admission.Email != null)
                            {
                                var stdEmail = new EmailViewModel()
                                {
                                    Email = admission.Email,
                                    RegNo = admission.RegNo
                                };
                                StudentEmailsBox.Add(stdEmail);
                            }

                            // DOES THE STUDENT APPLICANT EXIST?
                            var stdAdmission = await repoAdmission.GetByIdAsync(x => x.RegNo == admission.RegNo && x.JambRegNo == admission.JambRegNo);

                            if (stdAdmission == null) // DOES NOT EXIST
                            {
                                //ADD STUDENT TO APPLICANT LIST
                                repoAdmission.Add(admission);

                                string ms = admission.RegNo.ToString() + " was submitted succesfully";

                                //ADD RESPONSE TO UPLOAD RESPONSE
                                upRes.Message = ms;
                                uploadResponses.Add(upRes);
                                countDone += 1;
                            }
                            else // YES STUDENT EXIST
                            {
                                string ms = admission.RegNo.ToString() + "Duplicate Student Registration Number";
                                //ADD RESPONSE TO UPLOAD RESPONSE
                                upRes.Message = ms;
                                uploadResponses.Add(upRes);
                                countNotDone += 1;
                            }
                            startRow++;
                        }


                        var inventory = new Inventory()
                        {
                            User = User.Identity.Name,
                            Action = "The user uploaded new students record to the department."
                        };
                        repoInventory.Add(inventory);

                        System.IO.File.Delete(path);

                        string mail = "";

                        //SEND MAIL TO EACH ADMITTED STUDENT
                        foreach (var stdEmail in StudentEmailsBox)
                        {
                            //IdentityMessage ITidMesStd = new IdentityMessage()
                            //{
                            //    Destination = stdEmail.Email,
                            //    Subject = "Account Registration Link"
                            //};

                            string code = Guid.NewGuid().ToString() + DateTime.Now.ToString().Trim();
                            var callbackUrl = Url.Page(
                         "/Account/RegisterStudent",
                         pageHandler: null,
                         values: new { area = "Identity", regNo = stdEmail.RegNo, email = stdEmail.Email, code = code },
                         protocol: Request.Scheme);
                            //Url.Action("StudentUpload", "Account", new { regNo = stdEmail.RegNo, code = code });

                            string message = "Please complete your account registration by clicking <a href=\"" + callbackUrl + "\">here</a>";

                            //MailHelper sendMail = new MailHelper();
                            //ConfirmEmailSend sendMsg = sendMail.SendMail(ITidMesStd, message);

                            //ADD LINK TO HYPERLINKS
                            var link1 = new HyperLink()
                            {
                                Link = callbackUrl,
                                Email = stdEmail.Email,
                            };
                            repoHyperlink.Add(link1);
                        }

                        //INFORM THE UPLOADER OF THE UPLOAD MADE
                        var _user = User.Identity.Name;

                        //IdentityMessage ITidMes = new IdentityMessage()
                        //{
                        //    Destination = _user,
                        //    Subject = "Upload response"
                        //};

                        //GO THROUGH ALL THE RESPONSES AND SEND MAIL TO INSTITUTION ITF EMAIL
                        foreach (var m in uploadResponses)
                        {
                            mail += "<hr />" + m.Message;
                        }

                        //MailHelper sendMail1 = new();
                        //ConfirmEmailSend sendMsg1 = sendMail1.SendMail(ITidMes, mail);

                        //ADD LINK TO HYPERLINKS
                        var link = new HyperLink()
                        {
                            Link = mail,
                            Email = _user,
                        };

                        repoHyperlink.Add(link);

                        msg = $"{countDone} student(s) uploaded successfully and {countNotDone} failed, please check your email for more details!";
                    }
                    else
                    {
                        msg = "Please select a valid file, and Try again!";
                    }
                }
                else
                {
                    msg = "Sorry, you provided invalid data or failed to select your department " +
                        "when logged in, logout, login and try again!";
                }
            }
            catch (Exception ex)
            {
                msg = "Fatal Error, please try again or contact admin!";
            }
            TempData[v] = msg;

            return RedirectToAction("UploadStudent");
        }

        #endregion ADMIN UPLOAD STUDENT REGISTRATION NUMBER YEARLY AFTER ADMISSION

        #region - CREATE SESSIONS

        //SAVE AND UPDATE SESSION
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HOD,DepartmentICT,Admin,SchoolOwner")]
        public async Task<JsonResult> SaveSession(AcademicSessionVM model)
        {
            string response = "";
            try
            {
                if (ModelState.IsValid)
                {
                    var dbObj = await repoAcaSession.GetByIdAsync(o => o.Id == model.Id);

                    if (dbObj != null)
                    {
                        dbObj.Title = model.Title;

                        repoAcaSession.Update(dbObj);
                    }
                    else
                    {
                        AcademicSession newAcaSession = new()
                        {
                            Title = model.Title
                        };
                        var old = await repoAcaSession.GetByIdAsync(x => x.Title == model.Title);
                        if (old == null)
                        {
                            repoAcaSession.Add(newAcaSession);
                            response = "Operation Completed Successfully!";
                        }
                        else
                            response = model.Title + " already exist!";
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

                    response = error;
                }
            }
            catch (Exception ex)
            {
                response = "Fatal Error, please try again or contact admin!";
            }

            TempData[v] = response;

            return Json(response);
        }

        //ADD AND EDIT SESSION
        [Authorize(Roles = "HOD,DepartmentICT,Admin,SchoolOwner")]
        public async Task<IActionResult> AddEditSession(string ID)
        {
            AcademicSessionVM model = new();
            try
            {
                if (!String.IsNullOrEmpty(ID))
                {
                    var obj = await repoAcaSession.GetByIdAsync(o => o.Id == ID);
                    if (obj != null)
                    {
                        model.Id = obj.Id;
                        model.Title = obj.Title;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!";
                return RedirectToAction("Index");
            }


            return PartialView("AddEditSession", model);
        }

        #endregion - END - CREATE SESSIONS

        #region -   GET STUDENTS BOTH REGISTERED AND NON

        [Authorize(Roles = "HOD,DepartmentICT")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Students(AcademicSessionVM model)
        {
            string msg = string.Empty;
            var allStudents = new List<GetStudentsVm>();

            try
            {
                string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
                string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;

                if (!ModelState.IsValid || string.IsNullOrEmpty(sessionDeptId) && string.IsNullOrEmpty(sessionEmpId))
                    TempData[v] = "Please select the required option, or failed to select your department when logged in, logout, login and try again!";
                else
                {

                    //  GET STUDENT_ADMISSION WITH SAME DEPARTMENT_ID
                    var _admissions = await repoAdmission.GetByQueryAsync(stds => stds.DepartmentId == sessionDeptId
                                                                          && stds.Session == model.Title);


                    if (_admissions.Any())
                        foreach (var adm in _admissions)
                        {
                            //  GET REGISTRATION DETAILS
                            var std = await repoStd.GetByIdAsync(x => x.RegNo == adm.RegNo);
                            if (std != null)
                                allStudents.Add(new GetStudentsVm
                                {
                                    Name = std.Name,
                                    MatricNo = std.RegNo,
                                    JambReg = std.JambRegNo,
                                    YOA = adm.Session,
                                    IsRegistered = true
                                });
                            else
                                allStudents.Add(new GetStudentsVm
                                {
                                    Name = "NOT REGISTERED",
                                    MatricNo = adm.RegNo,
                                    JambReg = adm.JambRegNo,
                                    YOA = adm.Session,
                                    IsRegistered = false
                                });
                        }
                }
            }
            catch (Exception)
            {
                TempData[v] = "Fatal Error, please contact admin!";
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());


            ViewBag.Sessions = await repoAcaSession.GetAll();
            ViewBag.Count = allStudents.Count;
            ViewBag.AllStudents = allStudents.OrderBy(x => x.Name).ToList();

            model = new AcademicSessionVM();
            return View(model);
        }

        #endregion



    }
}
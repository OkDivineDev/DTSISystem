using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces;
using ClosedXML.Excel;
using DataAccessLayer.Enum;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using WebUI.DTOs;

namespace WebUI.Controllers
{
    public class CourseAllocationController : Controller
    {
        private readonly LocalInfrastructure local = new();
        private readonly PopNotification popNotification;
        private readonly string csAllocApprove = "CsAllApp";
        private readonly string v = "Msg";
        private readonly IRepository<CourseAllocation> repoCourseAllo;
        private readonly INotyfService notyfService;
        private readonly IRepository<Inventory> repoInventory;
        private readonly IHttpContextAccessor context;
        private readonly IRepository<Lecturer> repoLec;
        private readonly IRepository<Department> repoDept;
        private readonly IRepository<AcademicSession> repoSession;
        private readonly IRepository<CourseBank> repoCourse;
        private readonly IRepository<UserInterface> repoUserInterface;

        public CourseAllocationController(IRepository<CourseAllocation> _repoCourseAllo,
                                          INotyfService _notyfService,
                                          IRepository<Inventory> _repoInventory,
                                          IHttpContextAccessor _context,
                                          IRepository<Lecturer> _repoLec,
                                          IRepository<Department> _repoDept,
                                          IRepository<AcademicSession> _repoSession,
                                          IRepository<CourseBank> _repoCourse,
                                          IRepository<UserInterface> _repoUserInterface)
        {
            repoCourseAllo = _repoCourseAllo;
            notyfService = _notyfService;
            repoInventory = _repoInventory;
            context = _context;
            repoLec = _repoLec;
            repoDept = _repoDept;
            repoSession = _repoSession;
            repoCourse = _repoCourse;
            repoUserInterface = _repoUserInterface;
            popNotification = new PopNotification(notyfService);
        }

        [Authorize(Roles = "HOD, Lecturer")]
        public async Task<IActionResult> Index(SearchAllocationVm model)
        {
            try
            {

                string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
                string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;


                bool isHod = await IsHod(sessionEmpId, sessionDeptId);
                ViewBag.IsHod = isHod;

                IEnumerable<CourseAllocation> dbCourseAlloc = null;
                IEnumerable<CourseAllocationVm> courseAlloc = null;

                //var getUser = await repoUserInterface.GetByIdAsync(x => x.Email == User.Identity.Name);

                //var emp = await repoEmp.GetByIdAsync(emp => emp.UserId == getUser.UserId);
                if (sessionEmpId == null || sessionDeptId == null)
                {
                    TempData[v] = "Error, logout, login with your department selected and try again!";
                    return RedirectToAction("Index", "AdminManager");
                }
                else
                {
                    var isHodToDept = await repoLec.GetByIdAsync(x => x.Id == sessionEmpId);
                    var dept = await repoDept.GetByIdAsync(x => x.Id == sessionDeptId && x.HODUserId == isHodToDept.UserId);

                    if (User.IsInRole("HOD") && dept != null)
                    {
                        if (!string.IsNullOrEmpty(model._Session))
                            dbCourseAlloc = await repoCourseAllo.GetByQueryAsync(x => x.Session == model._Session && x.DepartmentID == sessionDeptId);
                        else
                            dbCourseAlloc = await repoCourseAllo.GetByQueryAsync(x => x.DepartmentID == sessionDeptId);

                        if (dbCourseAlloc.Any())
                        {
                            _ = dbCourseAlloc.OrderBy(x => x.Session);

                            courseAlloc = await PopulateCourseAllocation(dbCourseAlloc, true, model._Session);
                        }
                        else if (!string.IsNullOrEmpty(model._Session))
                        {
                            courseAlloc = await PopulateCourseAllocation(dbCourseAlloc, true, model._Session);
                        }
                    }
                    else if (User.IsInRole("DepartmentICT"))
                    {
                        if (!string.IsNullOrEmpty(model._Session))
                            dbCourseAlloc = await repoCourseAllo.GetByQueryAsync(x => x.Session == model._Session && x.DepartmentID == sessionDeptId);
                        else
                            dbCourseAlloc = await repoCourseAllo.GetByQueryAsync(x => x.DepartmentID == sessionDeptId);

                        if (dbCourseAlloc.Any())
                        {
                            _ = dbCourseAlloc.OrderBy(x => x.Session);

                            courseAlloc = await PopulateCourseAllocation(dbCourseAlloc, false, model._Session);
                        }
                        else if (!string.IsNullOrEmpty(model._Session))
                        {
                            courseAlloc = await PopulateCourseAllocation(dbCourseAlloc, true, model._Session);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model._Session))
                            dbCourseAlloc = await repoCourseAllo.GetByQueryAsync(x => x.Session == model._Session && x.LecturerID == sessionEmpId);
                        else
                            dbCourseAlloc = await repoCourseAllo.GetByQueryAsync(x => x.LecturerID == sessionEmpId);

                        if (dbCourseAlloc.Any())
                        {
                            _ = dbCourseAlloc.OrderBy(x => x.Session);

                            courseAlloc = await PopulateCourseAllocation(dbCourseAlloc, true, model._Session);
                        }
                        else if (!string.IsNullOrEmpty(model._Session))
                        {
                            courseAlloc = await PopulateCourseAllocation(dbCourseAlloc, true, model._Session);
                        }
                    }



                    ViewBag.CourseAllocations = courseAlloc;
                    ViewBag.Count = courseAlloc != null ? courseAlloc.Count() : 0;

                    ViewBag.Sessions = await repoSession.GetAll();
                }
            }
            catch (Exception ex)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!";
                return RedirectToAction("Index", "AdminManager");
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "HOD")]
        public async Task<IActionResult> AllocateCourse(string? ID)
        {
            try
            {
                string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
                string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;


                //var getUser = await repoUserInterface.GetByIdAsync(x => x.Email == User.Identity.Name);

                //var emp = await repoEmp.GetByIdAsync(emp => emp.UserId == getUser.UserId);
                if (sessionEmpId == null || sessionDeptId == null)
                {
                    TempData[v] = "Error, logout, login with your department selected and try again!";
                    return RedirectToAction("Index", "AdminManager");
                }
                else
                {
                    var courses = await repoCourse.GetByQueryAsync(x => x.DepartmentID == sessionDeptId);
                    ViewBag.Courses = courses.Select(x => new { Id = x.Id, Code = x.Code });

                    var sessions = await repoSession.GetAll();
                    ViewBag.Sessions = sessions.Select(x => new { Title = x.Title });

                    var employees = await repoLec.GetByQueryAsync(x => x.DepartmentID == sessionDeptId);
                    ViewBag.Employees = employees.Select(x => new { Id = x.Id, Name = x.Name });
                }
            }
            catch (Exception ex)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!";
                return RedirectToAction("Index", "AdminManager");
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());

            if (!string.IsNullOrEmpty(ID))
            {
                var dbCsAlloc = await repoCourseAllo.GetByIdAsync(x => x.Id == ID);
                if (dbCsAlloc != null)
                {
                    var editCsAlloc = new CourseAllocationVm
                    {
                        CourseID = dbCsAlloc.CourseID,
                        LecturerID = dbCsAlloc.LecturerID,
                        Session = dbCsAlloc.Session,
                        Semester = (SemesterEnum)dbCsAlloc.Semester,
                        DepartmentID = dbCsAlloc.DepartmentID,
                        Id = dbCsAlloc.Id
                    };

                    return View(editCsAlloc);
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateCourse(CourseAllocationVm model)
        {
            string msg = null;
            try
            {
                string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
                string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;

                if (ModelState.IsValid && sessionEmpId != null && sessionDeptId != null)
                {
                    int semester = Convert.ToInt32(model.Semester);

                    var lectEmp = await repoLec.GetByIdAsync(x => x.Id == model.LecturerID);


                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        //UPDATE RECORD
                        var oldCsAlloc = await repoCourseAllo.GetByIdAsync(x => x.Id == model.Id);

                        oldCsAlloc.Session = model.Session;
                        oldCsAlloc.Semester = (int)model.Semester;
                        oldCsAlloc.CourseID = model.CourseID;
                        oldCsAlloc.DepartmentID = sessionDeptId;
                        oldCsAlloc.LecturerID = model.LecturerID;

                        if (repoCourseAllo.Update(oldCsAlloc))
                            msg = "Operation completed successfully!";
                        else
                            msg = "Error, Update failed, please try again!";
                    }
                    else
                    {
                        var newCsAlloc = new CourseAllocation
                        {
                            DepartmentID = sessionDeptId,
                            LecturerID = model.LecturerID,
                            CourseID = model.CourseID,
                            Semester = semester,
                            Session = model.Session
                        };

                        var checkDB = await repoCourseAllo.GetByIdAsync(
                                                           x => x.CourseID == model.CourseID &&
                                                           x.Session == model.Session);

                        if (checkDB != null)
                        {
                            TempData[v] = $"The selected course has been initially allocated to {lectEmp.Name} for the same session selected!";
                            return RedirectToAction("AllocateCourse");
                        }

                        if (repoCourseAllo.Add(newCsAlloc))
                            msg = "Operation completed successfully!";
                        else
                            msg = "Error, Operation failed, try again!";

                        var course = await repoCourse.GetByIdAsync(x => x.Id == model.CourseID);

                        var newInventory = new Inventory
                        {
                            Action = User.Identity.Name + $" tried to allocate course {course.Code} to {lectEmp.Name}\n" + msg,
                            User = User.Identity.Name,
                        };

                        repoInventory.Add(newInventory);
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
                            error = "Logout, login again with your department selected and try again!";
                    }
                    msg = error;
                }
            }
            catch (Exception)
            {
                msg = "Fatal Error, Please try again or contact the Admin!";
            }

            if (msg != null)
                popNotification.Notyf(msg);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "HOD")]
        public async Task<IActionResult> Approve(CourseAllocationApprovalVm model)
        {
            var csAllocToApprove = new List<CourseAllocationVm>();
            try
            {
                string getCsAllocApp = context.HttpContext.Session.GetString(csAllocApprove);

                if (getCsAllocApp != null)
                {
                    List<CourseAllocationVm> csAllocs = JsonConvert.DeserializeObject<List<CourseAllocationVm>>(getCsAllocApp);

                    csAllocToApprove = csAllocs;
                }

                if (ModelState.IsValid)
                {
                    var dbCsAllocs = await repoCourseAllo.GetByQueryAsync(x => x.Session == model.Session && x.Semester == (int)model.Semester);
                    if (dbCsAllocs.Any())
                    {
                        csAllocToApprove = new List<CourseAllocationVm>();

                        foreach (var csAlloc in dbCsAllocs)
                        {
                            var course = await repoCourse.GetByIdAsync(x => x.Id == csAlloc.CourseID);
                            var emp = await repoLec.GetByIdAsync(x => x.Id == csAlloc.LecturerID);

                            if (!csAlloc.Approved)
                                csAllocToApprove.Add(new CourseAllocationVm
                                {
                                    Id = csAlloc.Id,
                                    CourseID = course.Code,
                                    LecturerID = emp.Name,
                                    Session = csAlloc.Session,
                                    Semester = (SemesterEnum)csAlloc.Semester
                                });
                        }

                        // HOW TO SET SESSION UP
                        string csAllocToApproveString = JsonConvert.SerializeObject(csAllocToApprove.ToList());
                        context.HttpContext.Session.SetString(csAllocApprove, csAllocToApproveString);
                    }
                    else
                        csAllocToApprove = new List<CourseAllocationVm>();
                }
                else
                {
                    string error = "";
                    foreach (var err in ModelState)
                    {
                        if (err.Value.Errors.Any())
                            error += "[" + err.Key + "]: " + err.Value.Errors.FirstOrDefault().ErrorMessage;
                    }

                    TempData[v] = error;
                }
            }
            catch (Exception ex)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!";
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());

            ViewBag.Count = csAllocToApprove.Count;
            ViewBag.CourseAllocationToApprove = csAllocToApprove?.ToList();

            var sessions = await repoSession.GetAll();
            _ = sessions.OrderBy(x => x.Title);

            ViewBag.Sessions = sessions.ToList();

            return View();
        }

        [Authorize(Roles = "HOD")]
        public async Task<IActionResult> ApproveNow()
        {
            string msg = null;
            try
            {
                string getCsAllocApp = context.HttpContext.Session.GetString(csAllocApprove);

                if (getCsAllocApp != null)
                {
                    List<CourseAllocationVm> csAllocs = JsonConvert.DeserializeObject<List<CourseAllocationVm>>(getCsAllocApp);
                    if (csAllocs.Any())
                    {
                        foreach (var csAlloc in csAllocs)
                        {
                            var dbCsAlloc = await repoCourseAllo.GetByIdAsync(x => x.Id == csAlloc.Id);
                            if (dbCsAlloc != null)
                            {
                                dbCsAlloc.Approved = true;

                                if (repoCourseAllo.Update(dbCsAlloc))
                                    msg += "<br />[" + csAlloc.CourseID + " > " + csAlloc.LecturerID + "] " + "APPROVED!";
                                else
                                    msg += "[" + csAlloc.CourseID + " > " + csAlloc.LecturerID + "] " + "FAILED!";
                            }
                            else
                            {
                                msg += "[" + csAlloc.CourseID + " > " + csAlloc.LecturerID + "] " + "NOT FOUND!";
                            }
                        }
                    }
                    else
                        msg = "Error, Operation failed, try again!";
                }
            }
            catch (Exception ex)
            {
                msg = "Fata Error, please try again or contact admin!";
            }

            if (!string.IsNullOrEmpty(msg))
                popNotification.Notyf(msg);

            // CLEAR SESSION
            if (context.HttpContext.Session.GetString(csAllocApprove) != null)
                context.HttpContext.Session.Remove(csAllocApprove);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "HOD")]
        [HttpGet]
        public async Task<IActionResult> RemoveCourseAllocationApproval(string ID)
        {
            string msg = null;
            try
            {
                if (!string.IsNullOrEmpty(ID))
                {
                    string getCsAllocApp = context.HttpContext.Session.GetString(csAllocApprove);

                    if (getCsAllocApp != null)
                    {
                        List<CourseAllocationVm> csAllocs = JsonConvert.DeserializeObject<List<CourseAllocationVm>>(getCsAllocApp);

                        var toRemove = csAllocs.FirstOrDefault(x => x.Id == ID);
                        if (toRemove != null)
                        {
                            csAllocs.Remove(toRemove);

                            // HOW TO SET SESSION UP
                            string csAllocToApproveString = JsonConvert.SerializeObject(csAllocs.ToList());
                            context.HttpContext.Session.SetString(csAllocApprove, csAllocToApproveString);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = "Fatal Error, please try again or contact admin!";
            }
            if (!string.IsNullOrEmpty(msg))
                popNotification.Notyf(msg);

            return RedirectToAction("Approve");
        }

        [Authorize(Roles = "HOD")]
        [HttpGet]
        public async Task<IActionResult> RemoveCourseAllocation(string ID)
        {
            string msg = null;
            try
            {
                if (!string.IsNullOrEmpty(ID))
                {
                    var dbCsAlloc = await repoCourseAllo.GetByIdAsync(x => x.Id == ID);
                    if (dbCsAlloc != null)
                        if (repoCourseAllo.Delete(dbCsAlloc))
                            msg = "The selected record has been successfully deleted!";
                }
            }
            catch (Exception ex)
            {
                msg = "Fatal Error, please try again or contact admin!";
            }
            if (!string.IsNullOrEmpty(msg))
                popNotification.Notyf(msg);

            return RedirectToAction("Index");
        }

        #region ADMIN UPLOAD COURSE ALLOCATION RECORDS

        [Authorize(Roles = "HOD")]
        public async Task<IActionResult> UploadCourseAllocation()
        {
            var sessions = await repoSession.GetAll();
            ViewBag.Sessions = sessions.Select(x => new { Title = x.Title });

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "HOD")]
        public async Task<ActionResult> UploadCourseAllocation(ExcelUploadFile model)
        {
            string msg = "";
            int countDone = 0;
            int countNotDone = 0;

            ErrorMessageViewModel errorMessage = new();

            try
            {

                string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
                string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;


                if (sessionDeptId == null && sessionEmpId == null || model.Semester == null || string.IsNullOrEmpty(model.Session))
                {
                    msg = "Please make sure you're providing both session and semester!";

                }
                else if (ModelState.IsValid)
                {
                    //CHECK IF UPLOAD FILE IS EMPTY
                    if (model.ExcelFile.Name == null || model.ExcelFile.Length < 1)
                    {
                        msg = "Please select excel file and Try again!";
                    }
                    else if (model != null && model.ExcelFile.Length > 0 && model.ExcelFile.FileName.EndsWith("xls") || model.ExcelFile.FileName.EndsWith("xlsx"))
                    {
                        string deptID = "";
                        string userId = null;

                        //var user = await repoUserInterface.GetByIdAsync(x => x.Email == User.Identity.Name);
                        //userId = user.UserId;

                        //var emp = await repoEmp.GetByIdAsync(x => x.UserId == userId);
                        //if (emp == null)
                        //{
                        //    TempData[v] = "Error, You cannot continue with the operation!";
                        //    return RedirectToAction("Index", "Home");
                        //}

                        deptID = sessionDeptId;

                        string path = Path.Combine("wwwroot", "TempUpload", model.ExcelFile.FileName.Replace(" ", "_"));

                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);

                        await local.LocalImageStore(path, model.ExcelFile);

                        var uploadResponses = new List<UploadResponse>();

                        XLWorkbook xLWorkbook = new(path);

                        int startRow = 3;
                        while (xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 1).GetString() != "")
                        {
                            CourseAllocation newCsAlloc = new();

                            UploadResponse upRes = new();
                            string ms = "";
                            string csCode = string.Empty;

                            try
                            {
                                newCsAlloc.DepartmentID = deptID;
                                newCsAlloc.Semester = (int)model.Semester;
                                newCsAlloc.Session = model.Session;

                                csCode = xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 2).GetString();
                                string email = xLWorkbook.Worksheets.Worksheet(1).Cell(startRow, 3).GetString();

                                var course = await repoCourse.GetByIdAsync(x => x.Code.ToLower() == csCode.ToLower());
                                if (course == null)
                                {
                                    ms = $"Unable to create Course Allocation record due to {csCode} does not exist, " +
                                        $"please verify course table and try again!";
                                    //ADD RESPONSE TO UPLOAD RESPONSE
                                    upRes.Message = ms;
                                    uploadResponses.Add(upRes);
                                    countNotDone += 1;
                                }
                                else if (!IsEmailValid(email))
                                {
                                    ms = $"Unable to create Course Allocation record due to invalid email address - {email}!";
                                    //ADD RESPONSE TO UPLOAD RESPONSE
                                    upRes.Message = ms;
                                    uploadResponses.Add(upRes);
                                    countNotDone += 1;
                                }
                                else
                                {
                                    var lectUser = await repoUserInterface.GetByIdAsync(x => x.Email == email);
                                    var lectEmp = await repoLec.GetByIdAsync(x => x.UserId == lectUser.UserId);

                                    if (lectEmp != null && lectEmp.DepartmentID == deptID)
                                    {
                                        //  ADD NEW COURSE ALLOCATION TO DB
                                        csCode = course.Code;
                                        newCsAlloc.LecturerID = lectEmp.Id;
                                        newCsAlloc.CourseID = course.Id;

                                        var oldSameRec = await repoCourseAllo.GetByIdAsync(x => x.CourseID == newCsAlloc.CourseID
                                                                                           && x.Session == newCsAlloc.Session);
                                        if (oldSameRec != null)
                                        {
                                            ms = $"The selected course and session has been allocated already check the table to find out!";
                                            countNotDone += 1;
                                        }
                                        else
                                        {
                                            if (repoCourseAllo.Add(newCsAlloc))
                                            {
                                                ms = $"{newCsAlloc.CourseID} was successfully allocated to {email}!";
                                                countDone += 1;
                                            }
                                            else
                                            {
                                                ms = $"{newCsAlloc.CourseID} was not allocated to {email}, something went wrong, try again!";
                                                countNotDone += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ms = $"The selected course was not allocated to {email} because the lecturer is not part of the department!";
                                        countNotDone += 1;
                                    }

                                    //ADD RESPONSE TO UPLOAD RESPONSE
                                    upRes.Message = ms;
                                    uploadResponses.Add(upRes);

                                    TempData[v] = $"{countDone} uploaded and {countNotDone} failed, please check your mail for details!";
                                    return RedirectToAction("Index");
                                }
                            }
                            catch (Exception ex)
                            {
                                ms = $"{csCode ?? "COURSE UNKNOWN"} encountered Fatal Error while processing, please contact admin!";

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
                            Action = "The user uploaded new course allocation records to the department."
                        };

                        repoInventory.Add(inventory);

                        //DELETE THE FILE
                        System.IO.File.Delete(path);

                        string mail = "";

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

                        // msg = "(" + countDone.ToString() + ") course allocation(s) uploaded successfully and (" + countNotDone + ") failed, please check your email for more details!";
                    }
                    else
                    {
                        msg = "Please select a valid file, and Try again!";
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
                msg = "Fatal Error, Please try again or contact Admin!";
            }

            var sessions = await repoSession.GetAll();
            ViewBag.Sessions = sessions.Select(x => new { Title = x.Title });

            if (!string.IsNullOrEmpty(msg))
                popNotification.Notyf(msg);

            return View(model);
        }


        #endregion ADMIN UPLOAD COURSE ALLOCATION RECORDS

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

        private async Task<IEnumerable<CourseAllocationVm>> PopulateCourseAllocation(IEnumerable<CourseAllocation> dbCourseAllocations, bool isLec, string session)
        {
            var users = await repoUserInterface.GetAll();
            var courseAllocationVm = new List<CourseAllocationVm>();

            foreach (var csAlloc in dbCourseAllocations)
            {
                var semester = csAlloc.Semester == 1 ? SemesterEnum.First : SemesterEnum.Second;
                var course = await repoCourse.GetByIdAsync(x => x.Id == csAlloc.CourseID);
                var lect = await repoLec.GetByIdAsync(x => x.Id == csAlloc.LecturerID);
                var dept = await repoDept.GetByIdAsync(x => x.Id == lect.DepartmentID);

                var newcsAlloc = new CourseAllocationVm()
                {
                    Approved = csAlloc.Approved,
                    CourseID = course.Code,
                    Semester = semester,
                    Session = csAlloc.Session,
                    Id = csAlloc.Id,
                    LecturerID = lect.Name,
                    DepartmentID = dept.Code
                };

                courseAllocationVm.Add(newcsAlloc);
            }

            if (isLec)
            {
                var user = users.FirstOrDefault(x => x.Email == User.Identity.Name);

                IEnumerable<CourseAllocation> hodCsAllocs = null;

                var hodEmps = await repoLec.GetByQueryAsync(x => x.UserId == user.UserId);

                if (hodEmps.Any())
                    foreach (var hodEmp in hodEmps)
                    {
                        if (!string.IsNullOrEmpty(session))
                            hodCsAllocs = await repoCourseAllo.GetByQueryAsync(x => x.LecturerID == hodEmp.Id && x.Session == session);
                        else
                            hodCsAllocs = await repoCourseAllo.GetByQueryAsync(x => x.LecturerID == hodEmp.Id);

                        if (hodCsAllocs.Any())
                            foreach (var csAlloc in hodCsAllocs)
                            {
                                var semester = csAlloc.Semester == 1 ? SemesterEnum.First : SemesterEnum.Second;
                                var course = await repoCourse.GetByIdAsync(x => x.Id == csAlloc.CourseID);
                                var lect = await repoLec.GetByIdAsync(x => x.Id == csAlloc.LecturerID);

                                var exist = courseAllocationVm.FirstOrDefault(x => x.Id == csAlloc.Id);
                                if (exist == null)
                                {
                                    var dept = await repoDept.GetByIdAsync(x => x.Id == csAlloc.DepartmentID);
                                    courseAllocationVm.Add(
                                    new CourseAllocationVm()
                                    {
                                        Approved = csAlloc.Approved,
                                        CourseID = course.Code,
                                        Semester = semester,
                                        Session = csAlloc.Session,
                                        Id = csAlloc.Id,
                                        LecturerID = lect.Name,
                                        DepartmentID = dept.Code
                                    });
                                }
                            }

                    }
            }
            return courseAllocationVm.ToList();
        }
    }
}
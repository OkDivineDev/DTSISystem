using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.DTOs;

namespace WebUI.Controllers
{
    [Authorize]
    public class AssignmentManagerController : Controller
    {
        private readonly string v = "Msg";
        private readonly IRepository<CourseBank> repoCourse;
        private readonly IRepository<Lecturer> repoLec;
        private readonly IHttpContextAccessor context;
        private readonly INotyfService notyfService;
        private readonly IRepository<Student> repoStd;
        private readonly IRepository<Assignment> repoAssign;
        private readonly IRepository<StudentAssignment> repoStdAssign;
        private readonly IRepository<CourseAllocation> repoCSAlloc;
        private readonly IRepository<UserInterface> repoUser;
        private readonly PopNotification popNotification;

        public AssignmentManagerController(IRepository<CourseBank> _repoCourse,
                                       IRepository<Lecturer> _repoLec,
                                       IHttpContextAccessor _context,
                                       INotyfService _notyfService,
                                       IRepository<Student> _repoStd,
                                       IRepository<Assignment> _repoAssign,
                                       IRepository<StudentAssignment> _repoStdAssign,
                                       IRepository<CourseAllocation> _repoCSAlloc,
                                       IRepository<UserInterface> _repoUser)
        {
            repoCourse = _repoCourse;
            repoLec = _repoLec;
            context = _context;
            notyfService = _notyfService;
            repoStd = _repoStd;
            repoAssign = _repoAssign;
            repoStdAssign = _repoStdAssign;
            repoCSAlloc = _repoCSAlloc;
            repoUser = _repoUser;
            popNotification = new PopNotification(notyfService);
        }

        #region ASSIGNMENT CRUD

        //LIST ALL COURSE
        [HttpGet]
        [Authorize(Roles = "Lecturer,Student")]
        public async Task<IActionResult> Index()
        {
            var assignments = new List<AssignmentVM>();

            try
            {
                ViewBag.Assignment = new AssignmentVM();
                ViewBag.Assignments = new List<AssignmentVM>();
                ViewBag.Count = 0;

                string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
                string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;

                if (!string.IsNullOrEmpty(sessionEmpId) && User.IsInRole("Lecturer"))
                {
                    var lecAssignments = await repoAssign.GetByQueryAsync(x => x.LecturerID == sessionEmpId);

                    if (lecAssignments.Any())
                    {
                        lecAssignments = lecAssignments.OrderByDescending(x => x.CreatedOn).ToList();

                        foreach (var assign in lecAssignments)
                        {
                            var course = await repoCourse.GetByIdAsync(c => c.Id == assign.CourseID);

                            assignments.Add(new AssignmentVM
                            {
                                Id = assign.Id,
                                Code = assign.Code,
                                Date = assign.CreatedOn.ToString("dd/MM/yyyy, HH:mm:ss"),
                                CourseID = assign.CourseID,
                                QuestionList = assign.Questions.Split("\r\n").ToList()
                            });
                        }
                    }
                }

            }
            catch (Exception)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!";
                return RedirectToAction("Index", "Home");
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());

            if (assignments.Any())
            {
                ViewBag.Assignments = assignments.ToList();
                ViewBag.Count = assignments.Count;
            }

            return View();
        }

        //SAVE AND UPDATE ALL COURSE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<JsonResult> Index(AssignmentVM model)
        {
            string response = "";
            try
            {
                if (ModelState.IsValid)
                {

                    var course = await repoCourse.GetByIdAsync(x => x.Id == model.CourseID);

                    Assignment newAssignment = new()
                    {
                        Code = $"{course.Code.Replace(" ", "")}{DateTime.Now:HHmmssMMyy}",
                        CourseID = model.CourseID,
                        LecturerID = model.LecturerID,
                        Questions = model.Questions,
                        SubmissionDate = model.SubmissionDate
                    };

                    repoAssign.Add(newAssignment);


                    response = "Operation Completed Successfully!";
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
                response = "FATAL ERROR, PLEASE TRY AGAIN, IF PERSIST CONTACT ADMIN";
            }

            TempData[v] = response;

            return Json(response);
        }

        //ADD AND EDIT COURSE
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> AddEditAssignment()
        {
            AssignmentVM model = new();

            var csAllocs = new List<CourseAllocation>();
            var courses = new List<CourseBankViewModel>();

            string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
            string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;

            try
            {

                if (!string.IsNullOrEmpty(sessionEmpId) && !string.IsNullOrEmpty(sessionDeptId))
                {
                    model.LecturerID = sessionEmpId;
                }
                else
                {
                    TempData[v] = "Faild, please logout and login again by selecting the department.";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception)
            {
                TempData[v] = "FATAL ERROR, PLEASE TRY AGAIN, IF PERSIST CONTACT ADMIN";
                return RedirectToAction("Index");
            }

            var dbCsAllocs = await repoCSAlloc.GetByQueryAsync
                            (x => x.DepartmentID == sessionDeptId &&
                            x.LecturerID == sessionEmpId);

            if (dbCsAllocs.Any())
                foreach (var cs in dbCsAllocs)
                {
                    var course = await repoCourse.GetByIdAsync(x => x.Id == cs.CourseID);

                    courses.Add(new CourseBankViewModel
                    {
                        ID = course.Id,
                        Code = $"{course.Code} - {course.Title}"
                    });
                }

            ViewBag.Courses = courses.ToList();

            return PartialView("AddEditAssignment", model);
        }

        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> DeleteAssignment(string ID)
        {
            try
            {
                if (!String.IsNullOrEmpty(ID))
                {
                    var objDel = await repoAssign.GetByIdAsync(o => o.Id == ID);
                    if (objDel != null)
                    {
                        repoAssign.Delete(objDel);
                        TempData[v] = "Operation Completed Successfully!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData[v] = "FATAL ERROR, PLEASE TRY AGAIN, IF PERSIST CONTACT ADMIN";
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> SubmittedAssignments(string ID)
        {
            List<StudentAssignmentVM> studentAssignmentVM = new();

            try
            {

                if (!String.IsNullOrEmpty(ID))
                {
                    var dbStdAssignments = await repoStdAssign.GetByQueryAsync(o => o.Id == ID);
                    if (dbStdAssignments.Any())
                    {
                        foreach (var stdAss in dbStdAssignments)
                        {
                            studentAssignmentVM.Add(new StudentAssignmentVM
                            {
                                StudentRegNo = stdAss.StudentRegNo,
                                AssignmentID = stdAss.AssignmentID,
                                DateSubmitted = stdAss.CreatedOn.ToString("dd/MM/yyyy,HH:mm:ss")
                            });
                        }

                    }
                    ViewBag.Count = studentAssignmentVM.Count;
                }
            }
            catch (Exception ex)
            {
                TempData[v] = "FATAL ERROR, PLEASE TRY AGAIN, IF PERSIST CONTACT ADMIN";
                return RedirectToAction("Index");
            }

            return PartialView("SubmittedAssignments", studentAssignmentVM.OrderBy(x => x.DateSubmitted).ToList());
        }

        [Authorize(Roles = "Student,Lecturer")]
        public IActionResult RetrieveAssignment()
        {
            var model = new SearchAssignmentVm();

            return PartialView("RetrieveAssignment", model);
        }



        [Authorize(Roles = "Student,Lecturer")]
        [HttpPost]
        public IActionResult RetrieveAssignment(SearchAssignmentVm model)
        {
            string msg = string.Empty;
            string code = string.Empty;

            try
            {

                if (ModelState.IsValid)
                {

                    code = model.Code;
                }
                else
                    return PartialView("NOTFOUND");

            }
            catch (Exception)
            {
                msg = "Fatal Error, please try again or contact the admin!";
            }

            if (!string.IsNullOrEmpty(msg))
                popNotification.Notyf(msg);


            return Json(code);

        }


        [Authorize(Roles = "Student,Lecturer")]
        public async Task<IActionResult> RetrievedAssignment(string id)
        {
            string msg = string.Empty;

            var model = new AssignmentVM();

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var dbRetrievedAss = await repoAssign.GetByIdAsync(x => x.Code == id);
                    if (dbRetrievedAss != null)
                    {
                        model = new AssignmentVM
                        {
                            Code = dbRetrievedAss.Code,
                            Date = dbRetrievedAss.CreatedOn.ToString("dd/MM/yyyy-HH:mm:ss"),
                            Id = dbRetrievedAss.Id,
                            QuestionList = dbRetrievedAss?.Questions.Split("\r\n").ToList(),
                            CourseID = dbRetrievedAss.CourseID,
                            SubmissionDate = dbRetrievedAss.SubmissionDate
                        };

                        msg = "Assignment Loaded!";

                        return PartialView("RetrievedAssignment", model);

                    }
                }

            }
            catch (Exception)
            {
                msg = "Fatal Error, try again or contact the admin!";
            }


            if (!string.IsNullOrEmpty(msg))
                popNotification.Notyf(msg);

            return Json("NOTFOUND");

            //return Json(msg);


        }



        #endregion ASSIGNMENT CRUD


    }
}
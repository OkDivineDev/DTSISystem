using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.DTOs;

namespace WebUI.Controllers
{
    [Authorize]
    public class CourseManagerController : Controller
    {
        private readonly string v = "Msg";
        private readonly IRepository<CourseBank> repoCourse;
        private readonly IRepository<Lecturer> repoLec;
        private readonly IHttpContextAccessor context;
        private readonly INotyfService notyfService;
        private readonly IRepository<Student> repoStd;
        private readonly IRepository<Department> repoDept;
        private readonly IRepository<UserInterface> repoUser;
        private readonly PopNotification popNotification;

        public CourseManagerController(IRepository<CourseBank> _repoCourse,
                                       IRepository<Lecturer> _repoLec,
                                       IHttpContextAccessor _context,
                                       INotyfService _notyfService,
                                       IRepository<Student> _repoStd,
                                       IRepository<Department> _repoDept,
                                       IRepository<UserInterface> _repoUser)
        {
            repoCourse = _repoCourse;
            repoLec = _repoLec;
            context = _context;
            notyfService = _notyfService;
            repoStd = _repoStd;
            repoDept = _repoDept;
            repoUser = _repoUser;
            popNotification = new PopNotification(notyfService);
        }

        #region COURSE CRUD

        //LIST ALL COURSE
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
                string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;


                bool isHod = await IsHod(sessionEmpId, sessionDeptId);
                ViewBag.IsHod = isHod;


                //var user = await repoUser.GetByIdAsync(x => x.Email == User.Identity.Name);
                var coursesVm = new List<CourseBankViewModel>();
                //var std = await repoStd.GetByIdAsync(x => x.UserId == user.UserId);
                //var emp = await repoEmp.GetByIdAsync(x => x.UserId == user.UserId);

                string deptID = null;
                string deptName = null;

                if (sessionDeptId != null)
                {
                    var department = await repoDept.GetByIdAsync(x => x.Id == sessionDeptId);
                    if (department != null)
                    {
                        TempData["deptId"] = department.Id;
                        deptID = department.Id;
                        deptName = department.Name;
                    }
                }
                //else if (emp != null)
                //{
                //    var department = await repoDept.GetByIdAsync(x => x.Id == emp.DepartmentID);
                //    if (department != null)
                //    {
                //        TempData["deptId"] = department.Id;
                //        deptID = department.Id;
                //        deptName = department.Name;
                //    }
                //}

                if (!String.IsNullOrEmpty(deptID) && !String.IsNullOrEmpty(deptName))
                {
                    var getCourses = await repoCourse.GetByQueryAsync(c => c.DepartmentID == deptID);
                    if (getCourses.Any())
                    {
                        foreach (var course in getCourses)
                        {
                            coursesVm.Add(new
                                CourseBankViewModel
                            {
                                ID = course.Id,
                                Code = course.Code,
                                Department = deptName,
                                Unit = course.Unit,
                                DepartmentID = deptID,
                                Title = course.Title
                            });
                        }
                    }
                }

                ViewBag.Count = coursesVm.Count;
                ViewBag.AllCoursesx = coursesVm.ToList();
            }
            catch (Exception ex)
            {
                TempData[v] = ex.Message.ToString();
            }

            if (TempData[v] != null)
            {
                popNotification.Notyf(TempData[v].ToString());
            }

            return View();
        }

        //SAVE AND UPDATE ALL COURSE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HOD,DepartmentICT")]
        public async Task<JsonResult> Index(CourseBankViewModel model)
        {
            string response = "";
            try
            {


                if (ModelState.IsValid)
                {
                    var dbObj = await repoCourse.GetByIdAsync(o => o.Code == model.Code && o.Unit == model.Unit || o.Id == model.ID);

                    if (dbObj != null)
                    {
                        dbObj.Code = model.Code;
                        dbObj.Unit = model.Unit;
                        dbObj.Title = model.Title;
                        dbObj.DepartmentID = model.DepartmentID;
                        repoCourse.Update(dbObj);
                    }
                    else
                    {
                        CourseBank objCo = new()
                        {
                            Code = model.Code,
                            Unit = model.Unit,
                            Title = model.Title,
                            DepartmentID = model.DepartmentID
                        };
                        repoCourse.Add(objCo);
                    }

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
        [Authorize(Roles = "HOD,DepartmentICT")]
        public async Task<IActionResult> AddEditCourse(string ID)
        {
            CourseBankViewModel model = new();
            try
            {
                if (!String.IsNullOrEmpty(ID))
                {
                    var obj = await repoCourse.GetByIdAsync(o => o.Id == ID);
                    if (obj != null)
                    {
                        model.ID = obj.Id;
                        model.Code = obj.Code;
                        model.Unit = obj.Unit;
                        model.Title = obj.Title;
                        model.DepartmentID = obj.DepartmentID;
                    }
                }
                else
                {
                    //GET USER DEPARTMENT
                    model.DepartmentID = TempData["deptId"].ToString();
                }
            }
            catch (Exception ex)
            {
                TempData[v] = "FATAL ERROR, PLEASE TRY AGAIN, IF PERSIST CONTACT ADMIN";
                return RedirectToAction("Index");
            }

            ViewBag.Departments = await repoDept.GetAll();
            return PartialView("AddEditCourse", model);
        }

        [Authorize(Roles = "HOD,DepartmentICT")]
        public async Task<IActionResult> DeleteCourse(string ID)
        {
            try
            {
                if (!String.IsNullOrEmpty(ID))
                {
                    var objDel = await repoCourse.GetByIdAsync(o => o.Id == ID);
                    if (objDel != null)
                    {
                        repoCourse.Delete(objDel);
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

        #endregion COURSE CRUD

        private async Task<bool> IsHod(string? empId, string? deptId)
        {
            var isHodToDept = await repoLec.GetByIdAsync(x => x.Id == empId);
            var dept = await repoDept.GetByIdAsync(x => x.Id == deptId && x.HODUserId == isHodToDept.UserId);

            if (dept != null)
                return true;
            else
                return false;
        }


    }
}
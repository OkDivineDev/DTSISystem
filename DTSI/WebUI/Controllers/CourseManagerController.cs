using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLayer.Interfaces;
using DataAccessLayer.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
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
        private readonly IRepository<CourseOutLine> repoCSOutline;
        private readonly IRepository<Department> repoDept;
        private readonly IRepository<UserInterface> repoUser;
        private readonly PopNotification popNotification;

        public CourseManagerController(IRepository<CourseBank> _repoCourse,
                                       IRepository<Lecturer> _repoLec,
                                       IHttpContextAccessor _context,
                                       INotyfService _notyfService,
                                       IRepository<Student> _repoStd,
                                       IRepository<CourseOutLine> _repoCSOutline,
                                       IRepository<Department> _repoDept,
                                       IRepository<UserInterface> _repoUser)
        {
            repoCourse = _repoCourse;
            repoLec = _repoLec;
            context = _context;
            notyfService = _notyfService;
            repoStd = _repoStd;
            repoCSOutline = _repoCSOutline;
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

                var coursesVm = new List<CourseBankViewModel>();

                string deptID = null;
                string deptName = null;
                if (sessionDeptId != null)
                {
                    //if (User.IsInRole("Student"))
                    //{
                    var department = await repoDept.GetByIdAsync(x => x.Id == sessionDeptId);
                    if (department != null)
                    {
                        TempData["deptId"] = department.Id;
                        deptID = department.Id;
                        deptName = department.Name;
                    }
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
                    //}
                    //else
                    //{

                    //    var department = await repoDept.GetByIdAsync(x => x.Id == sessionDeptId);
                    //    if (department != null)
                    //    {
                    //        TempData["deptId"] = department.Id;
                    //        deptID = department.Id;
                    //        deptName = department.Name;
                    //    }

                    //    var getCourses = await repoCourse.GetByQueryAsync(c => c.DepartmentID == deptID);
                    //    if (getCourses.Any())
                    //    {
                    //        foreach (var course in getCourses)
                    //        {
                    //            coursesVm.Add(new
                    //                CourseBankViewModel
                    //            {
                    //                ID = course.Id,
                    //                Code = course.Code,
                    //                Department = deptName,
                    //                Unit = course.Unit,
                    //                DepartmentID = deptID,
                    //                Title = course.Title
                    //            });
                    //        }
                    //    }

                    //}

                    if (!string.IsNullOrEmpty(sessionEmpId))
                    {
                        bool isHod = await IsHod(sessionEmpId, sessionDeptId) == true;
                        ViewBag.IsHod = isHod;
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
                    var dbObj = await repoCourse.GetByIdAsync(o => o.Code == model.Code || o.Id == model.ID);

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




        #region COURSE-GUIDE CRUDE
        //LIST ALL COURSE OUTLINES
        [HttpGet]
        [Authorize(Roles = "HOD,DepartmentICT,Student")]
        public async Task<IActionResult> CourseOutLine(SearchCourseOutlineVM model, string courseId)
        {
            try
            {

                var courseOutLineVm = new List<CourseOutLineVm>();


                if (ModelState.IsValid || !string.IsNullOrEmpty(courseId))
                {


                    var getCourseOutLines = await repoCSOutline.GetByQueryAsync(c => c.CourseId == model.CourseID || c.CourseId == courseId);
                    getCourseOutLines = getCourseOutLines.OrderBy(x => x.SN);


                    if (getCourseOutLines.Any())
                    {
                        foreach (var csOutLine in getCourseOutLines)
                        {

                            var course = await repoCourse.GetByIdAsync(x => x.Id == csOutLine.CourseId);

                            courseOutLineVm.Add(new
                                CourseOutLineVm
                            {
                                Id = csOutLine.Id,
                                CourseId = course.Code,
                                OutLine = csOutLine.OutLine,
                                SN = csOutLine.SN
                            });
                        }
                    }
                }

                ViewBag.Count = courseOutLineVm.Count;
                ViewBag.AllCourseOutLines = courseOutLineVm.OrderBy(x => x.CourseId).OrderBy(x => x.OutLine).ToList();
                ViewBag.Courses = repoCSOutline.GetAll();

            }
            catch (Exception ex)
            {
                TempData[v] = "Fatal Error, please contact Admin!";
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HOD,DepartmentICT")]
        public async Task<JsonResult> CourseOutLine(CourseOutLineVm model)
        {
            string response = "";
            try
            {


                if (ModelState.IsValid)
                {
                    string[] ids = Array.Empty<string>();
                    string[] courseIds = Array.Empty<string>();
                    string[] outlines = Array.Empty<string>();

                    //SPLIT FIRST
                    if (!string.IsNullOrEmpty(model.Id) && model.Id.Contains(','))
                        ids = model.Id.Split(',');

                    if (!string.IsNullOrEmpty(model.CourseId) && model.CourseId.Contains(','))
                        courseIds = model.CourseId.Split(',');

                    if (model.OutLine.Contains('\n'))
                        outlines = model.OutLine.Split("\r\n");

                    if (ids.Any() && courseIds.Any())
                    {
                        int totIds = ids.Length;
                        int done = 0, notDone = 0;

                        for (int i = 0; i < totIds; i++)
                        {

                            if (!string.IsNullOrEmpty(outlines[i]))
                            {
                                var dbOb = await repoCSOutline.GetByIdAsync(o => o.Id == ids[i] && o.CourseId == courseIds[i]);

                                if (dbOb != null)
                                {
                                    dbOb.OutLine = outlines[i];

                                    if (repoCSOutline.Update(dbOb))
                                        done += 1;
                                    else
                                        notDone += 1;

                                }

                            }

                            response = $"Out of {outlines.Length} outlines {done} were added while {notDone} failed!";
                        }

                    }
                    else
                    {
                        var dbObj = await repoCSOutline.GetByIdAsync(o => o.Id == model.Id);

                        if (dbObj != null)
                        {
                            dbObj.OutLine = model.OutLine;

                            if (repoCSOutline.Update(dbObj))
                                response = "Operation Completed Successfully!";
                            else
                                response = "Operation Completed Successfully!";
                        }
                        else
                        {

                            int totCourseOutline = outlines.Length;
                            int done = 0, notDone = 0;

                            

                         

                            for (int i = 0; i < totCourseOutline; i++)
                            {
                                var allCsOutline = await repoCSOutline.GetByQueryAsync(x => x.CourseId == model.CourseId);
                                int lastSN = 0;

                                if (allCsOutline.Any())
                                {
                                    lastSN = allCsOutline.OrderBy(x => x.SN).LastOrDefault().SN;
                                }

                                if (!string.IsNullOrEmpty(outlines[i]))
                                {
                                    CourseOutLine objCo = new()
                                    {
                                        CourseId = model.CourseId,
                                        OutLine = outlines[i],
                                        SN = lastSN + 1
                                    };

                                    if (repoCSOutline.Add(objCo))
                                        done += 1;
                                    else
                                        notDone += 1;
                                }

                            }

                            response = $"Out of {totCourseOutline} outlines {done} were added while {notDone} failed!";
                        }

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
                response = "FATAL ERROR, PLEASE TRY AGAIN, IF PERSIST CONTACT ADMIN";
            }

            TempData[v] = response;

            return Json(response);
        }


        //ADD AND EDIT COURSE
        [Authorize(Roles = "HOD,DepartmentICT")]
        public async Task<IActionResult> AddEditCourseOutLine(string? ID)
        {
            CourseOutLineVm model = new();
            try
            {
               
                if (!string.IsNullOrEmpty(ID))
                {
                    var obj = await repoCSOutline.GetByIdAsync(o => o.Id == ID);
                    if (obj != null)
                    {
                        model.Id = obj.Id;
                        model.CourseId = obj.CourseId;
                        model.OutLine = obj.OutLine;
                    }
                }
                             

            }
            catch (Exception ex)
            {
                TempData[v] = "FATAL ERROR, PLEASE TRY AGAIN, IF PERSIST CONTACT ADMIN";
                return RedirectToAction("CourseOutLine");
            }

            ViewBag.Courses = await repoCourse.GetAll();

            return PartialView("AddEditCourseOutLine", model);
        }


        [Authorize(Roles = "HOD,DepartmentICT")]
        public async Task<IActionResult> DeleteCourseOutLine(string ID)
        {
            try
            {
                if (!string.IsNullOrEmpty(ID))
                {
                    var objDel = await repoCSOutline.GetByIdAsync(o => o.Id == ID);
                    if (objDel != null)
                    {
                        repoCSOutline.Delete(objDel);
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

        #endregion









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
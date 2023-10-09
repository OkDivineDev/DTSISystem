using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.DTOs;

namespace WebUI.Controllers
{
    [Authorize(Roles = "Student,HOD,DeaprtmentICT,Lecturer")]
    public class ChatController : Controller
    {
        private readonly IRepository<CourseBank> repoCourse;
        private readonly IRepository<Lecturer> repoLec;
        private readonly IHttpContextAccessor context;
        private readonly INotyfService notyfService;
        private readonly IRepository<Student> repoStd;
        private readonly IRepository<CourseAllocation> repoCSAlloc;
        private readonly IRepository<CourseOutLine> repoCSOutline;
        private readonly IRepository<Department> repoDept;
        private readonly IRepository<Chat> repoChat;
        private readonly PopNotification popNotification;
        private readonly IRepository<UserInterface> repoUser;
        private readonly string v = "Msg";
        private readonly LocalInfrastructure local = new();


        public ChatController(IRepository<CourseBank> _repoCourse,
                                       IRepository<Lecturer> _repoLec,
                                       IHttpContextAccessor _context,
                                       INotyfService _notyfService,
                                       IRepository<Student> _repoStd,
                                       IRepository<CourseAllocation> _repoCSAlloc,
                                       IRepository<CourseOutLine> _repoCSOutline,
                                       IRepository<Department> _repoDept,
                                       IRepository<Chat> _repoChat,
                                       IRepository<UserInterface> _repoUser)
        {
            repoCourse = _repoCourse;
            repoLec = _repoLec;
            context = _context;
            notyfService = _notyfService;
            repoStd = _repoStd;
            repoCSAlloc = _repoCSAlloc;
            repoCSOutline = _repoCSOutline;
            repoDept = _repoDept;
            repoChat = _repoChat;
            repoUser = _repoUser;
            popNotification = new PopNotification(notyfService);
        }

        public async Task<IActionResult> Index(string courseId)
        {
            var chatsModel = new List<ChatVM>();


            try
            {
                var user = await repoUser.GetByIdAsync(x => x.Email == User.Identity.Name);


                var chats = await repoChat.GetByQueryAsync(x => x.Subject.Contains(courseId));
                var _chats = chats.OrderBy(date => date.CreatedOn);

                if (_chats.Any())
                    foreach (var chat in _chats)
                    {

                        var csID_csOID = chat.Subject.Split(',');

                        var course = await repoCourse.GetByIdAsync(x => x.Id == csID_csOID[0]);
                        var outLine = await repoCSOutline.GetByIdAsync(x => x.Id == csID_csOID[1]);

                        string userID = string.Empty;

                        var std = await repoStd.GetByIdAsync(x => x.UserId == chat.UserID);
                        var lect = await repoLec.GetByIdAsync(x => x.UserId == chat.UserID);

                        if (std != null)
                            userID = std.RegNo;

                        else
                            userID = lect.Name;

                        chatsModel.Add(new ChatVM
                        {
                            Id = chat.Id,
                            Message = chat.Message,
                            UserId = userID,
                            CourseId = course.Code,
                            OutlineId = outLine.SN.ToString(),
                            Image = chat.Image,
                            Date_Time = chat.CreatedOn,
                            IsYours = chat.UserID == user.UserId
                        });

                        //courseId = csID_csOID[0];
                    }
            }
            catch (Exception)
            {
                TempData[v] = "Fatal Error, try again or contact admin!";
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());


            ViewBag.CourseId = courseId;
            TempData["courseId"] = courseId;

            return View(chatsModel.ToList());
        }


        //[Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> AddEditStudentChat(string ID)
        {
            string courseId = string.Empty;

            if (TempData["courseId"] != null)
                courseId = TempData["courseId"].ToString();

            StudentChatVm model = new();
            try
            {
                if (!String.IsNullOrEmpty(ID))
                {
                    var obj = await repoChat.GetByIdAsync(o => o.Id == ID);
                    if (obj != null)
                    {
                        string[] courseIDs_OutlineIDs = obj.Subject.Split(',');

                        model.Id = obj.Id;
                        model.CourseId = courseIDs_OutlineIDs[0];
                        model.OutlineId = courseIDs_OutlineIDs[1];
                        model.Message = obj.Message;
                    }
                }

            }
            catch (Exception ex)
            {
                TempData[v] = "FATAL ERROR, PLEASE TRY AGAIN, IF PERSIST CONTACT ADMIN";

                return RedirectToAction("Index", new { courseId });
            }

            model.CourseId = courseId;

            var outlines = await repoCSOutline.GetByQueryAsync(x => x.CourseId == courseId);

            ViewBag.Outlines = outlines.OrderBy(x => x.SN).Select(x => new { x.Id, Outline = $"{x.SN}-" + x.OutLine });

            return PartialView("AddEditStudentChat", model);
        }


        [HttpPost]
        public async Task<IActionResult> StudentChat(StudentChatVm model)
        {
            string response = string.Empty;
            string courseId = string.Empty;

            try
            {
                string image = string.Empty;

                if (ModelState.IsValid)
                {
                    if (model.Image != null)
                        image = await ProcessChatImage(model.Image) ?? null;

                    //  PROCESS CHAT DATA AND SAVE

                    var user = await repoUser.GetByIdAsync(x => x.Email == User.Identity.Name);

                    if (string.IsNullOrEmpty(model.Id))
                    {
                        var newChat = new Chat
                        {
                            UserID = user.UserId,
                            Subject = $"{model.CourseId},{model.OutlineId}",
                            Message = model.Message,
                            Image = image
                        };

                        if (repoChat.Add(newChat))
                            response = "Sent Successfully";
                        else
                            response = "Not Sent, Try again";
                    }
                    else
                    {
                        var oldChat = await repoChat.GetByIdAsync(x => x.Id == model.Id);
                        if (oldChat != null)
                        {
                            oldChat.Subject = $"{model.CourseId},{model.OutlineId}";
                            oldChat.Message = model.Message;

                            if (!string.IsNullOrEmpty(image))
                            {
                                string path = Path.Combine("wwwroot", "Media", "ChatImage", oldChat.Image);

                                if (System.IO.File.Exists(path))
                                    System.IO.File.Delete(path);
                            }


                            oldChat.Image = image;

                            if (repoChat.Update(oldChat))
                                response = "Resent Successfully";
                            else
                                response = "Not resent, Try again";
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
            catch (Exception)
            {
                response = "Fatal Error, Please try again or contact admin!";
            }

            TempData[v] = response;

            //if (TempData["courseId"] != null)
            //    courseId = TempData["courseId"].ToString();

            return RedirectToAction("Index", new { model.CourseId });
        }

        [Authorize(Roles = "HOD,DepartmentICT")]
        public async Task<IActionResult> DeleteChat(string ID)
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







        public async Task<IActionResult> LoadCourses()
        {
            List<CourseBankViewModel> coursesVm = new();

            try
            {
                string? sessionDeptId = context.HttpContext.Session.GetString("SessionDeptId") ?? null;
                string? sessionEmpId = context.HttpContext.Session.GetString("SessionEmpId") ?? null;

                if (User.IsInRole("Lecturer") && !string.IsNullOrEmpty(sessionEmpId))
                {
                    var allocCs = await repoCSAlloc.GetByQueryAsync(x => x.LecturerID == sessionEmpId);
                    if (allocCs.Any())
                        foreach (var allocC in allocCs)
                        {
                            var course = await repoCourse.GetByIdAsync(x => x.Id == allocC.CourseID);

                            coursesVm.Add(new CourseBankViewModel
                            {
                                ID = course.Id,
                                Code = course.Code,
                                Title = course.Title
                            });
                        }
                }
                else if (User.IsInRole("Student") && !string.IsNullOrEmpty(sessionDeptId))
                {
                    var user = await repoUser.GetByIdAsync(x => x.Email == User.Identity.Name);
                    var std = await repoStd.GetByIdAsync(x => x.UserId == user.UserId);
                    string level = std.Level.ToString().Substring(0, 1);

                    int levelInitial = Convert.ToInt32(level);


                    var courses = await repoCourse.GetByQueryAsync(x => x.DepartmentID == sessionDeptId && Convert.ToInt32(x.Code.Substring(4, 1)) <= levelInitial);


                    if (courses.Any())
                        foreach (var cs in courses)
                        {
                            coursesVm.Add(new CourseBankViewModel
                            {
                                ID = cs.Id,
                                Code = cs.Code,
                                Title = cs.Title
                            });
                        }
                }

            }
            catch (Exception)
            {
                TempData[v] = "Fatal Error, please try again or contact the admin!";
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());

            return View(coursesVm.OrderBy(x => x.Code));
        }

        public async Task<List<CourseOutLineVm>> GetOutline(string courseId)
        {
            var csOutlines = new List<CourseOutLineVm>();
            try
            {
                if (!string.IsNullOrEmpty(courseId))
                {
                    var outlines = await repoCSOutline.GetByQueryAsync(x => x.CourseId == courseId);
                    if (outlines.Any())
                        foreach (var cs in outlines)
                        {
                            csOutlines.Add(new CourseOutLineVm
                            {
                                Id = cs.Id,
                                OutLine = $"{cs.SN}. " + cs.OutLine
                            });
                        }
                }

            }
            catch (Exception)
            {
                return csOutlines.ToList();
            }
            return csOutlines.ToList();
        }

        [NonAction]
        private async Task<String> ProcessChatImage(IFormFile file)
        {
            string image = string.Empty;

            //  CHECK THERE'S IMAGE OBJECT
            if (file.FileName.ToLower().EndsWith("jpg") ||
                     file.FileName.ToLower().EndsWith("jpeg") ||
                     file.FileName.ToLower().EndsWith("png"))
            {

                //UPLOAD NOT EMPTY
                string imageName = $"{User.Identity.Name}_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}" + Path.GetExtension(file.FileName);


                string path = Path.Combine("wwwroot", "Media", "ChatImages", imageName.Replace(" ", "_"));

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                await local.LocalImageStore(path, file);

                image = imageName;
            }



            return image;
        }
    }
}

using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    [Authorize(Roles = "SuperAdmin,ICTAdmin,HOD,Admin,SchoolOwner")]
    public class AdminManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> rolemanager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly INotyfService notyfService;
        private readonly PopNotification popNotification;

        private readonly string v = "Msg";

        public AdminManagerController(RoleManager<IdentityRole> _rolemanager,
                                      UserManager<IdentityUser> _userManager,
                                      INotyfService _notyfService)
        {
            rolemanager = _rolemanager;
            userManager = _userManager;
            notyfService = _notyfService;
            popNotification = new PopNotification(notyfService);
        }

        public IActionResult Index()
        {
            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());

            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateRole(string? Id)
        {
            RoleViewModel editModel = new RoleViewModel();
            try
            {
                if (Id != null)
                {
                    var dbRole = await rolemanager.FindByIdAsync(Id);
                    if (dbRole != null)
                    {
                        editModel = new RoleViewModel()
                        {
                            RoleName = dbRole.Name,
                            Id = Id
                        };
                        return View(editModel);
                    }
                }
            }
            catch (Exception)
            {
                TempData[v] = "Fatal Error: Please try again or contact admin!";
            }

            if (TempData[v] != null)
            {
                popNotification.Notyf(TempData[v].ToString());
            }
            return View(editModel);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            IdentityResult result = new IdentityResult();
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id != null)
                    {
                        var dbRole = await rolemanager.FindByIdAsync(model.Id);
                        if (dbRole != null)
                        {
                            dbRole.Name = model.RoleName;
                            result = await rolemanager.UpdateAsync(dbRole);
                        }
                    }
                    else
                    {
                        IdentityRole identityRole = new IdentityRole()
                        {
                            Name = model.RoleName
                        };

                        result = await rolemanager.CreateAsync(identityRole);
                    }

                    if (result.Succeeded)
                    {
                        TempData[v] = "Action completed successfully!";
                        return RedirectToAction("Roles");
                    }
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            catch (Exception)
            {
                TempData[v] = "Fatal Error: Please try again or contact admin!";
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());

            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public IActionResult Roles()
        {
            try
            {
                var roles = rolemanager.Roles;
                if (roles.Any())
                {
                    var roleModels = new List<RoleViewModel>();
                    foreach (var role in roles)
                    {
                        roleModels.Add(new RoleViewModel()
                        {
                            RoleName = role.Name,
                            Id = role.Id
                        });
                    }
                    return View(roleModels);
                }
            }
            catch (Exception)
            {
                TempData[v] = "Fatal Error: Please try again or contact admin!";
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());

            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            if (Id != null)
            {
                var dbRole = await rolemanager.FindByIdAsync(Id);
                if (dbRole != null)
                {
                    var result = await rolemanager.DeleteAsync(dbRole);
                    if (result.Succeeded)
                        TempData[v] = "User removed from the role successfully!";
                    else
                        TempData[v] = "Error, operation was not successful!";
                }
            }
            return RedirectToAction("Roles");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public IActionResult AddUserToRole()
        {
            var model = new UserRoleViewModel();

            var users_roles = new RolesAndUsersViewModel();
            //  GET ALL ROLES
            var roles = rolemanager.Roles.ToList().Select(x =>
                new RoleViewModel()
                {
                    RoleName = x.Name,
                    Id = x.Id
                }
            );

            var users = userManager.Users.ToList();

            if (roles.Any() && users.Any())
            {
                ViewBag.AllUsers = users;
                ViewBag.Roles = roles;
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());

            return View(model);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddUserToRole(UserRoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var getUser = await userManager.FindByEmailAsync(model.Email);
                    if (getUser != null)
                    {
                        var getRole = await rolemanager.FindByIdAsync(model.RoleId);
                        if (getRole != null)
                        {
                            var result = await userManager.AddToRoleAsync(getUser, getRole.Name);
                            if (result.Succeeded)
                            {
                                TempData[v] = "Your have successfully added user to " +
                                "the selected role!";
                                return RedirectToAction("AddUserToRole");
                            }
                            else
                            {
                                foreach (IdentityError error in result.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Sorry, we were unable to add {model.Email} to the selected role " +
                                $"because the role does not exist anymore!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Sorry, we were unable to add {model.Email} to the selected role " +
                                $"because the user does not exist anymore!");
                    }
                }
            }
            catch (Exception)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!";
            }
            return RedirectToAction("AddUserToRole");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> UserRole()
        {
            try
            {
                var userInRoles = new List<UsersInRoles>();

                var roles = rolemanager.Roles.ToList();
                if (roles.Any())
                {
                    foreach (var role in roles)
                    {
                        var users = await userManager.GetUsersInRoleAsync(role.Name);
                        if (users.Any())
                        {
                            foreach (var user in users)
                            {
                                userInRoles.Add(new UsersInRoles()
                                {
                                    Email = user.Email,
                                    RoleName = role.Name
                                });
                            }
                        }
                    }
                }

                return View(userInRoles);
            }
            catch (Exception)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!";
            }

            if (TempData[v] != null)
                popNotification.Notyf(TempData[v].ToString());

            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RemoveFromRole(string RoleName, string Email)
        {
            try
            {
                if (RoleName != null && Email != null)
                {
                    var getRole = await rolemanager.FindByNameAsync(RoleName);
                    var getUser = await userManager.FindByEmailAsync(Email);

                    if (getRole != null || getUser != null)
                    {
                        var msg = "";

                        var result = await userManager.RemoveFromRoleAsync(getUser, RoleName);
                        if (result.Succeeded)
                        {
                            msg = "Your have successfully removed user from " +
                               $"{getRole.Name} role!";
                        }
                        else
                        {
                            foreach (IdentityError error in result.Errors)
                            {
                                msg += error.Description + "\n";
                            }
                        }

                        TempData[v] = msg;
                        return RedirectToAction("UserRole");
                    }
                }
            }
            catch (Exception)
            {
                TempData[v] = "Fatal Error, please try again or contact admin!";
            }
            return RedirectToAction("Index");
        }

        }
}
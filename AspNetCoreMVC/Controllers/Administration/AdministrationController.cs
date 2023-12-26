using AspNetCoreMVC.Models;
using AspNetCoreMVC.Models.AppDBContext.ExtendIdentityUser;
using AspNetCoreMVC.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AspNetCoreMVC.Controllers.Administration
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole()
                {
                    Name = model.RoleName
                };

                IdentityResult res = await roleManager.CreateAsync(identityRole);

                if (res.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }
                foreach (IdentityError err in res.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
      
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.Error = $"Role with id {id} not found";
                return View("NotFound");
            }
            EditRoleViewModel editRoleViewModel = new EditRoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name
            };

            IList<ApplicationUser> allUsers = userManager.Users.ToList();

            foreach (ApplicationUser user in allUsers)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    editRoleViewModel.Users.Add(user.UserName);
                }
            }

            return View(editRoleViewModel);

        }

        [HttpPost]
        

        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    ViewBag.Error = $"Role with id {model.Id} not found";
                    return View("NotFound");
                }

                role.Name = model.RoleName;

                var res = await roleManager.UpdateAsync(role);
                if (res.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in res.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.Error = $"Role with id {roleId} not found";
                return View("NotFound");
            }

            List<UserRoleFromRoleViewModel> model = new List<UserRoleFromRoleViewModel>();

            foreach (var user in userManager.Users.ToList())
            {
                UserRoleFromRoleViewModel userRoleViewModel = new UserRoleFromRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                userRoleViewModel.IsSelected = await userManager.IsInRoleAsync(user, role.Name);

                model.Add(userRoleViewModel);

            }
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleFromRoleViewModel> model, string roleId)
        {

            IdentityRole role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.Error = $"Role with id {roleId} not found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult identityResult = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    identityResult = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    identityResult = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (identityResult.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }


        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.Error = $"User with id {id} not found";
                return View("NotFound");
            }
            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            EditUserViewModel editUserViewModel = new EditUserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                City = user.city,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles
            };



            return View(editUserViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {


                var user = await userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    ViewBag.Error = $"User with id {model.Id} not found";
                    return View("NotFound");
                }

                user.Email = model.Email;
                user.UserName = model.UserName;
                user.city = model.City;

                IdentityResult res = await userManager.UpdateAsync(user);

                if (res.Succeeded)
                {
                    return RedirectToAction("ListUsers", "Administration");
                }
                else
                {
                    foreach (IdentityError error in res.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);

            if (user == null)
            {
                ViewBag.Error = $"User with id {Id} not found";
                return View("NotFound");
            }

            IdentityResult res = await userManager.DeleteAsync(user);

            if (res.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }
            else
            {
                foreach (IdentityError error in res.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            var users = userManager.Users;
            return View("ListUsers", users);
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.Error = $"Role with id {Id} not found";
                return View("NotFound");
            }

            try
            {
                IdentityResult res = await roleManager.DeleteAsync(role);

                if (res.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (IdentityError error in res.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                var roles = roleManager.Roles;
                return View("ListRoles", roles);
            }
            catch (DbUpdateException ex)
            {
                @ViewBag.ExceptionMessage = ex.Message;
                return View("Error");
            }

        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string UserId)
        {
            ViewBag.userId = UserId;
            var user = await userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ViewBag.Error = $"User with id {UserId} not found";
                return View("NotFound");
            }
            ViewBag.userId =  UserId ;

            List<UserRoleFromUserViewModel> model = new List<UserRoleFromUserViewModel>();

            foreach (IdentityRole role in roleManager.Roles.ToList())
            {
                model.Add(new UserRoleFromUserViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = await userManager.IsInRoleAsync(user, role.Name)
                });

            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(List<UserRoleFromUserViewModel> model, string UserId)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(UserId);
                if (user == null)
                {
                    ViewBag.Error = $"User with id {UserId} not found";
                    return View("NotFound");
                }

                var roles = await userManager.GetRolesAsync(user);
                var result = await userManager.RemoveFromRolesAsync(user, roles);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot remove user existing roles");
                    return View(model);
                }

                result = await userManager.AddToRolesAsync(user,
                    model.Where(x => x.IsSelected).Select(y => y.RoleName));

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add selected roles to user");
                    return View(model);
                }

            }
            return RedirectToAction("EditUser", new { id = UserId });
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaimsFromUserView(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.Error = $"User with id {userId} not found";
                return View("NotFound");
            }

            IList<Claim> existingClaims = await userManager.GetClaimsAsync(user);

            var model = new ManageUserClaimsFromUserViewModel()
            {
                UserId = userId,
            };

            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                if (existingClaims.Any(x => x.Type == claim.Type && x.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);

            }
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaimsFromUserView(ManageUserClaimsFromUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.Error = $"User with id {model.UserId} not found";
                return View("NotFound");
            }

            var claims = await userManager.GetClaimsAsync(user);

            var res = await userManager.RemoveClaimsAsync(user, claims);


            if (!res.Succeeded)
            {
                ModelState.AddModelError("", "Error while updating claim selection");
                return View(model);
            }


            // Add all the claims that are selected on the UI
            res = await userManager.AddClaimsAsync(user,
                model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

            if (!res.Succeeded)
            {
                ModelState.AddModelError("", "Error while updating claim selection");
                return View(model);
            }

            return RedirectToAction("EditUser", new { id = model.UserId });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

}

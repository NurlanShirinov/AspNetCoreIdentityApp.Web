using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> Index()
        {
           var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();

            
            return View(roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();

            }

            TempData["SuccessMessage"] = "Role Created";

            return RedirectToAction(nameof(RolesController.Index));
        }

        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);

            if(roleToUpdate == null)
            {
                throw new Exception("Updated role is not found");
            }

            return View(new RoleUpdateViewModel() { Id=roleToUpdate.Id, Name=roleToUpdate!.Name! });
        }

        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            if (roleToUpdate == null)
            {
                throw new Exception("Role is not found");
            }

            roleToUpdate.Name = request.Name;

            await _roleManager.UpdateAsync(roleToUpdate);

            ViewData["SuccessMessage"] = "Role updated succefully";
            return View();
        }

        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);

            if (roleToDelete == null)
            {
                throw new Exception("Role is not found");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }

            TempData["SuccessMessage"] = "Role deleted";

            return RedirectToAction(nameof(RolesController.Index));

        }

        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = (await _userManager.FindByIdAsync(id))!; // Useri aliriq

            ViewBag.userId = id;

            var roles = await _roleManager.Roles.ToListAsync(); // movcud olan rollari elde edirik

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var roleViewModelList = new List<AssigneRoleToUserViewModel>(); // geriye bir list qaytarmaliyiq


            foreach(var role in roles)
            {
                var assignRoleToUserViewModel = new AssigneRoleToUserViewModel() { Id = role.Id, Name = role.Name!};

                if (userRoles.Contains(role.Name!))
                {
                    assignRoleToUserViewModel.Exist = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId , List<AssigneRoleToUserViewModel> requestList)
        {
            var userToAssignRoles = (await _userManager.FindByIdAsync(userId))!;

            foreach (var role in requestList)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }

            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }
    }
}
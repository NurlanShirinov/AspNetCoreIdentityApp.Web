using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    //MemberController sadece istifadecilerin istifade ede bileceyiseyfelerde olacaq ve
    //[Authorize] tagin yazmaliyiq

    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var curerntUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            var userViewModel = new UserViewModel
            {
                UserName = curerntUser.UserName,
                Email = curerntUser.Email,
                PhoneNumber = curerntUser.PhoneNumber
            };
            return View(userViewModel);
        }
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, request.PasswordOld);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Old Pssword is wrong")
;            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.PasswordOld,request.PasswordNew);

            if(!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x => x.Description).ToList());
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser); //password deyishdiyi ucun SecuritySamp i update etmeliyik
            await _signInManager.SignOutAsync(); // Parolu deyishdikden sonra yeniden saxil olmasi ucun signout edirik.
            await _signInManager.PasswordSignInAsync(currentUser,request.PasswordNew,true,false); //burda ise yeni passwordu daxil olma passwordu teyin edirik.


            TempData["SuccessMessage"] = "Password change successfully!";

            return View();
        }
    }
}
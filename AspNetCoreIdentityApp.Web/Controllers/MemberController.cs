using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Security.Claims;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Service.Services;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    //MemberController sadece istifadecilerin istifade ede bileceyiseyfelerde olacaq ve
    //[Authorize] tagin yazmaliyiq

    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private readonly IMemberService _memberService;
        private string userName => User.Identity!.Name!;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _memberService.GetUserViewModelByUserNameAsync(userName));
        }

        public async Task Logout()
        {
            await _memberService.LogoutAsync();
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

            if (! await _memberService.CheckPasswordAsync(userName, request.PasswordOld))
            {
                ModelState.AddModelError(string.Empty, "Old Pssword is wrong");
            }

            var (isSucced,errors) = await _memberService.CheckPasswordAsync(userName, request.PasswordOld, request.PasswordNew);

            if (!isSucced)
            {
                ModelState.AddModelErrorList(errors);
                return View();
            }

            TempData["SuccessMessage"] = "Password change successfully!";

            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = _memberService.GetGenderSelectList();
            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var (isSucces , errors) =  await _memberService.EditUserAsync(request, userName);

            if (!isSucces)
            {
                ModelState.AddModelErrorList(errors!);
                return View();
            }

            TempData["SuccessMessage"] = "User info changed successfully!";

            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }
        public async Task<IActionResult> AccessDenied(string ReturnUrl)
        {
            string message = string.Empty;

            message = "Bu sayfayi gormye yetkiniz yokdur. Yetki almak icin yoneticiniz ile gorushe bilirsiniz";

            ViewBag.message = message;

            return View();
        }

        [HttpGet]
        public IActionResult Claims()
        {
            return View(_memberService.GetClaims(User));
        }

        [Authorize(Policy = "AnkaraPolicy")]
        [HttpGet]
        public IActionResult AnkaraPage()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {
            return View();
        }



        [Authorize(Policy = "ViolencePolicy")]
        [HttpGet]
        public IActionResult ViolencePage()
        {
            return View();
        }


    }
}
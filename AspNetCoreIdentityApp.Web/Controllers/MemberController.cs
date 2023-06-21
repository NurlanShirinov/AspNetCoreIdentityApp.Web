using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.Models.TwoFactorService;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Security.Claims;

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
        private readonly TwoFactorService _twoFactorService;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, TwoFactorService twoFactorService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _twoFactorService = twoFactorService;
        }

        public async Task<IActionResult> Index()
        {
            var curerentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;
            var userViewModel = new UserViewModel
            {
                UserName = curerentUser!.UserName,
                Email = curerentUser.Email,
                PhoneNumber = curerentUser.PhoneNumber,
                PictureUrl = curerentUser.Picture
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

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, request.PasswordOld);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Old Pssword is wrong");
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.PasswordOld, request.PasswordNew);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser); //password deyishdiyi ucun SecuritySamp i update etmeliyik
            await _signInManager.SignOutAsync(); // Parolu deyishdikden sonra yeniden saxil olmasi ucun signout edirik.
            await _signInManager.PasswordSignInAsync(currentUser, request.PasswordNew, true, false); //burda ise yeni passwordu daxil olma passwordu teyin edirik.

            TempData["SuccessMessage"] = "Password change successfully!";

            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender))); //Gender enum dan datalari aliriq


            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };

            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            currentUser.UserName = request.UserName;
            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.BirthDate = request.BirthDate;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;


            //requestden geleen shekilin null olub olmamasini yoxluyuruq.
            if (request.Picture != null && request.Picture.Length > 0)
            {
                // fileProvider uzerinden shekli saxlamaq isdediyimiz folderi aliriq.
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                //shekli save etmek ucun ina bir random ad veririk ve extension un aliriq. meselen .jpg .png
                string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";

                //wwwroot icerisinden  usepictures folderin aliriq.
                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create); //yeni bir file create edirik

                await request.Picture.CopyToAsync(stream); // requestden gelen shekili stream-a copy edirik.
                currentUser.Picture = randomFileName;

            }

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);

            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser); //Deyishiklikler oldugu ucun SecurityStamp deyishirik
            await _signInManager.SignOutAsync();

            if (request.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("birthdate", currentUser.BirthDate.Value.ToString()) });
            }
            else
            {
                await _signInManager.SignInAsync(currentUser, true);
            }

            TempData["SuccessMessage"] = "User info changed successfully!";

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };

            return View(userEditViewModel);
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
            var userClaimList = User.Claims.Select(x => new ClaimViewModel()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value
            }).ToList();

            return View(userClaimList);
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

        public async Task<IActionResult> TwoFactorWithAuthenticator()
        {
            var currentUser = _userManager.FindByNameAsync(User!.Identity!.Name!).Result;

            string unformattedKey = (await _userManager.GetAuthenticatorKeyAsync(currentUser!))!; // Yoxluyuq eger databse de userin key i varmi

            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(currentUser!); //Key yoxdursa yaradiriq

                unformattedKey = (await _userManager.GetAuthenticatorKeyAsync(currentUser!))!; // yaradilmish keyi yeniden aliriq.
            }

            AuthenticatorViewModel authenticatorViewModel = new AuthenticatorViewModel();

            authenticatorViewModel.SharedKey = unformattedKey;

            authenticatorViewModel.AuthenticatorUri = _twoFactorService.GenerateQrCodeUri(currentUser.Email, unformattedKey);

            return View(authenticatorViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> TwoFactorWithAuthenticator(AuthenticatorViewModel authenticatorVM)
        {
            var verificationCode = authenticatorVM.VerificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var currentUser = _userManager.FindByNameAsync(User!.Identity!.Name!).Result;

            //istifadecinin dogrulama kodunun yoxlanilmasi
            var is2FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(currentUser!, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (is2FaTokenValid)
            {
                currentUser!.TwoFactorEnabled = true;
                currentUser!.TwoFactor = (sbyte)TwoFactor.MicrosoftGoogle;

                //istifadeciniin telefonu yaninda olmadigi zaman applicationa cata bilmeyeceyi ucun bir dene kurtarici kodlardan istifade ederek girish edecek.
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(currentUser!, 5);

                TempData["recoveryCodes"] = recoveryCodes;
                TempData["message"] = "Iki adimli kimlik dogrulama tipiniz Microsoft/Google Authenticator olaraq belirlenmishdir.";

                return RedirectToAction("TwoFactorAuth");
            }
            else
            {
                ModelState.AddModelError("", "Girdiyiniz Dogrulama kodu yanlishdir");
                return View(authenticatorVM);
            }


            return View();
        }


        public IActionResult TwoFactorAuth()
        {
            var currentUser = _userManager.FindByNameAsync(User!.Identity!.Name!).Result;

            return View(new AuthenticatorViewModel() { TwoFactorType = (TwoFactor)currentUser!.TwoFactor });
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorAuth(AuthenticatorViewModel authenticatorVM)
        {

            var currentUser = _userManager.FindByNameAsync(User!.Identity!.Name!).Result;

            switch (authenticatorVM.TwoFactorType)
            {
                case TwoFactor.None:
                    currentUser.TwoFactorEnabled = false;
                    currentUser.TwoFactor = (sbyte)TwoFactor.None;
                    TempData["message"] = "Iki adimli kimlik dogrulama tipiniz hicbiri olaraq belirlenmishdir.";
                    break;
                case TwoFactor.Phone:
                    break;
                case TwoFactor.Email:
                    break;
                case TwoFactor.MicrosoftGoogle:
                    return RedirectToAction("TwoFactorWithAuthenticator");
                default:
                    break;
            }

            await _userManager.UpdateAsync(currentUser!);

            return View(authenticatorVM);
        }
    }
}
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _UserManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _UserManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl ??= Url.Action("Index", "Home"); // eger returnUrl null dursa Url.Action ishleyecek.

            var hasUser = await _UserManager.FindByEmailAsync(model.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email ve ya shifre yanlish");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca girish yapamazsiniz" });
                return View();
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelErrorList(new List<string>() { $"Email veya shifre yanlish", $"Basharisiz girish sayisi = {await _UserManager.GetAccessFailedCountAsync(hasUser)}" });
                return View();
            }

            if (hasUser.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(hasUser, model.RememberMe, new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });
            }
            return Redirect(returnUrl!);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var identityResult = await _UserManager.CreateAsync(new() { UserName = request.UserName, PhoneNumber = request.Phone, Email = request.Email }, request.PasswordConfirm);

            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                return View();
            }

            var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString()); // userin qeydiyyatdan kecdiyi gunun  uzerine 10 gun gelirik. Bu policy base authorization ucundur.
            var user = await _UserManager.FindByNameAsync(request.UserName); // policy claimi vermek isdediyimiz useri tapiriq
            var claimResult = await _UserManager.AddClaimAsync(user!, exchangeExpireClaim); // burda ise usere claimi add edirik.

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                return View();
            }
            TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıyla gerçekleşmişdir.";
            return RedirectToAction(nameof(HomeController.SignUp));
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
        {
            var hasUser = await _UserManager.FindByEmailAsync(request.Email!);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Bu Email adresine sahib kullanici bulunmamakdadir");
                return View();
            }

            string passwordResetToken = await _UserManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);

            TempData["SuccessMessage"] = "Shifre yenileme linki e posta adresinize gonderilmishdir";

            return RedirectToAction(nameof(ForgetPassword));
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                throw new Exception("The Error occured!");
            }

            var hasUser = await _UserManager.FindByIdAsync(userId.ToString()!);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "User does not excist");
                return View();
            }

            var result = await _UserManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password updated successfully";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
            }

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //Facebook ile login buttonu ucun yazilmish IActionResult
        public IActionResult FacebookLogin(string ReturnUrl)
        {
            string RedirectUrl = (Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl }))!; // RedirectUrl istifadecinin facebook seyfesinde gorduyu ishleri bitirdikden sonra geleceyi seyfesidi.
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", RedirectUrl); // burda biz tnimlayirik ki hansi providere gedecek. Burda bizim provieder Facebook dur.
            //ChallengeResult Parametre olarax ne qebul edirse itifadecini ora yonledirir;
            return new ChallengeResult("Google", properties);
        }


        //Google ile login buttonu ucun yazilmish IActionResult
        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string RedirectUrl = (Url.Action("Response", "Home", new { ReturnUrl = ReturnUrl }))!; // RedirectUrl istifadecinin facebook seyfesinde gorduyu ishleri bitirdikden sonra geleceyi seyfesidi.
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", RedirectUrl); // burda biz tnimlayirik ki hansi providere gedecek. Burda bizim provieder Facebook dur.
            //ChallengeResult Parametre olarax ne qebul edirse itifadecini ora yonledirir;
            return new ChallengeResult("Facebook", properties);
        }


        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")
        {
            ExternalLoginInfo info = (await _signInManager.GetExternalLoginInfoAsync())!; // userin loginin oldugu ile bagli bezi melumatlar verecek.

            //istifadecinin login olub olmadigin melumatlarin yoxluyuruq null dursa login seyfesine redirect edirik
            if (info == null)
                return RedirectToAction("Login");

            //eger info null deyilse resultu aliriq.
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

            //result succeed oldugu halda returnURl -e yonlendiririki. Return Url bizim facebook login olduqdan sonra geleceyimiz seyfedi
            if (result.Succeeded)
                return Redirect(ReturnUrl);

            AppUser user = new AppUser();

            // bildiyimiz kimi claimler userlerin melumatlarin ozlerinde saxlayirdilar. Eyni zamanda da third party authentication-dan gelen melumatlar Identity Api terefinden claimlere kocururlur.
            //ve bizde bu claimler uzerinden userlerin datalarini elde ede bilirik.

            user.Email = info.Principal.FindFirst(ClaimTypes.Email)!.Value;

            string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
            {
                string userName = info.Principal.FindFirst(ClaimTypes.Name)!.Value;
                userName = userName.Replace(' ', '-').ToLower() + ExternalUserId.Substring(0, 5).ToString(); // User qeydiyyatdan kecende facebookda olan username-i database de ola biler onun ucun userlerin id lerinin ilk 5 reqemin username lerinin qarshisina atiriq.  
                user.UserName = userName;
            }
            else
            {
                user.UserName = info.Principal.FindFirst(ClaimTypes.Email)!.Value;
            }

            // burdan istifadecini save etmeye bashliyiriq

            IdentityResult createResult = await _UserManager.CreateAsync(user);

            if (createResult.Succeeded)
            {
                IdentityResult loginResult = await _UserManager.AddLoginAsync(user, info); // burda biz useri Database-de AspNetUserLogins cedveline save edirik. eger biz bunu save etmesek identity api userin facebookdan geldiyini anliya bilmez ona gordede application duzgun ishlemez

                if (loginResult.Succeeded)
                {
                    // await _signInManager.SignInAsync(user, true);

                    await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true); // applicationda userin hardan login oldugun bileceyik
                    return Redirect(ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong. Login failed");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Something went wrong");
            }


            return RedirectToAction("Error");





        }
    }
}
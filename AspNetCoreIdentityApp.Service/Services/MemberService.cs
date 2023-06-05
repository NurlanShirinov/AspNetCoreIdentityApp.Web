using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }

        public async Task LogoutAsync()
        {
           await _signInManager.SignOutAsync();
        }

        public async Task<UserViewModel> GetUserViewModelByUserNameAsync(string userName)
        {
            var curerentUser = (await _userManager.FindByNameAsync(userName))!;
            return new UserViewModel
            {
                UserName = curerentUser!.UserName,
                Email = curerentUser.Email,
                PhoneNumber = curerentUser.PhoneNumber,
                PictureUrl = curerentUser.Picture
            };
        }

        public async Task<bool> CheckPasswordAsync(string userName, string oldPassword)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName));
            return await _userManager.CheckPasswordAsync(currentUser, oldPassword);
        }

        public async Task<(bool, IEnumerable<IdentityError>)> CheckPasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName));
            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

            if (!resultChangePassword.Succeeded)
            {
                return (false, resultChangePassword.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentUser); //password deyishdiyi ucun SecuritySamp i update etmeliyik
            await _signInManager.SignOutAsync(); // Parolu deyishdikden sonra yeniden saxil olmasi ucun signout edirik.
            await _signInManager.PasswordSignInAsync(currentUser, newPassword, true, false); //burda ise yeni passwordu daxil olma passwordu teyin edirik.

            return (true, null);



        }

        public async Task<UserEditViewModel> GetUserEditViewModelAsync(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            return new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };
        }

        public SelectList GetGenderSelectList()
        {
           return new SelectList(Enum.GetNames(typeof(Gender)));
        }

        public async Task<(bool , IEnumerable<IdentityError>?)> EditUserAsync(UserEditViewModel request, string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;

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
                return (false, updateToUserResult.Errors);
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

            return (true, null);

        }


       public  List<ClaimViewModel> GetClaims(ClaimsPrincipal principal)
        {
           return principal.Claims.Select(x => new ClaimViewModel()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value
            }).ToList();
        }

    }
}

using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

    }
}

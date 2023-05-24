using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            var errors = new List<IdentityError>();
            if (password!.ToLower().Contains(user.UserName!
                .ToLower()))  // nida isharesi qoyaraq null olmamasini bildiririk
            {
                errors.Add(new() { Code = "PasswordContainUserName", 
                                   Description = "Shifre alani kullanici adi iceremez" });
            }

            if (password!.ToLower().StartsWith("1234"))
            {
                errors.Add(new() { Code = "PasswordContain1234", 
                                   Description = "Shifre alani ardicil sayi iceremez" });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);

        }
    }
}

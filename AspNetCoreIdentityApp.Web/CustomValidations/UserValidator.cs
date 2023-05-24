using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();
            var isDigit = int.TryParse(user.UserName[0]!.ToString(), out _); // bu codda olan _ simvolu discartdir. yeni normalda bu kodda onun yerinde result olmali idi lakin userin adi reqemle bashladigi halda bize lazim oldamigi ucun bele yazdiq.

            if (isDigit)
            {
                errors.Add(new() { Code = "UserNameContainFirstLetterDigit", 
                    Description = "Kullanici adini ilk sayisal bir karakter iceremez " });
            }
            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}

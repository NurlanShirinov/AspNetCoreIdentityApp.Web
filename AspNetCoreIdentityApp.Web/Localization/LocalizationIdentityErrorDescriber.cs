using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Localization
{
    public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new() { Code = "DuplicateUserName", 
                Description = $"{userName} daha bir kullanici tarafindan alinmishdir." };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new() { Code = "DuplicateEmail", 
                Description = $"{email} addresi bashqa biri tarafindan alinmishdir.." };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new() { Code = "PasswordTooShort", 
                Description = $"Shifre en az 6 karakter olmalidir" };
        }
    }
}

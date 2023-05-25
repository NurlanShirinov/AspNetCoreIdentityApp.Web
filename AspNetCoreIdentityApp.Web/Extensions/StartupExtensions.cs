using AspNetCoreIdentityApp.Web.CustomValidations;
using AspNetCoreIdentityApp.Web.Localization;
using AspNetCoreIdentityApp.Web.Models;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {
            //AppRole hissesinde yazilan options kodlar Password validation optionsdur.
            services.AddIdentity<AppUser, AppRole>(options => {

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvmxyz123456789_";

                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false; //passwordda simvol olmasada olar
               // options.Password.RequireNonAlphanumeric = true; //passwordda en az 1 simvol olmalidir.!@#$%^&* kimi
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;




                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;

            }).AddPasswordValidator<PasswordValidator>()
            .AddUserValidator<UserValidator>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddErrorDescriber<LocalizationIdentityErrorDescriber>();

        }
    }
}

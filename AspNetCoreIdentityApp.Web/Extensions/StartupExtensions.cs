using AspNetCoreIdentityApp.Web.CustomValidations;
using AspNetCoreIdentityApp.Web.Localization;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(2); //Forget passwordda generate etdiyimiz toke 2 saat omur verdik ki sonradan etibarsiz olsun kimse reset password falan etmesin.
            });

            //AppRole hissesinde yazilan options kodlar Password validation optionsdur.
            services.AddIdentity<AppUser, AppRole>(options =>
            {
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
              .AddDefaultTokenProviders()
              .AddErrorDescriber<LocalizationIdentityErrorDescriber>();

        }
    }
}

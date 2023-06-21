using AspNetCoreIdentityApp.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class AuthenticatorViewModel
    {
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        [Display(Name = "Dogrulama kodunuz")]
        [Required(ErrorMessage = "Dogrulama kodu gereklidir")]
        public string VerificationCode { get; set; }


        [Display(Name = "Iki adimli kimlik dogrulama")]
        public TwoFactor TwoFactorType { get; set; }
    }
}

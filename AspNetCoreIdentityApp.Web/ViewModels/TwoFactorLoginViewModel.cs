using AspNetCoreIdentityApp.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class TwoFactorLoginViewModel
    {
        [Display(Name ="Dogrulama kodunuz")]
        [Required(ErrorMessage ="Dogrulama kodu bosh olamaz")]
        [StringLength(20,ErrorMessage ="Dogrulama kodunuz en fazla 8 haneli olabilir")]
        public string VerificationCode { get; set; }

        public bool IsRememberMe { get; set; }
        public bool IsRecoverCode { get; set; }

        public TwoFactor TwoFactorType { get; set; }

    }
}

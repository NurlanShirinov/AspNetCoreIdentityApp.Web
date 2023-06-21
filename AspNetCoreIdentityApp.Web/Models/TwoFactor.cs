using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Models
{
    public enum TwoFactor
    {
        [Display(Name ="Hic Biri")]
        None = 0,

        [Display(Name = "Telefon ile kimlik dogrulama")]
        Phone = 1,

        [Display(Name = "Email ile kimlik dogrulama")]
        Email = 2,

        [Display(Name = "Microsoft/Google Authenticator ile kimlik dogrulama")]
        MicrosoftGoogle = 3,
    }
}
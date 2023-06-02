using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Password alanı boş bırakılamaz")]
        [Display(Name = "New Password")]
        public string Password { get; set; } = null!;


        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Shifre ayni deyildir")]
        [Required(ErrorMessage = "Password repeat alanı boş bırakılamaz")]
        [Display(Name = "Repeat Password")]
        public string PasswordConfirm { get; set; }
    }
}

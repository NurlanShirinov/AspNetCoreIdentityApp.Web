using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class PasswordChangeViewModel
    {

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password alanı boş bırakılamaz")]
        [Display(Name = "Password : ")]
        [MinLength(6, ErrorMessage = "Entered password should be minumum  6 characters")]
        public string PasswordOld { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = " New password  column cannot be emtpy")]
        [Display(Name = "New Password : ")]
        [MinLength(6, ErrorMessage = "Entered password should be minumum  6 characters")]
        public string PasswordNew { get; set; } = null!;


        [DataType(DataType.Password)]
        [Compare(nameof(PasswordNew), ErrorMessage = "Shifre ayni deyildir")]
        [Required(ErrorMessage = "Re-enter password column cannot be empty")]
        [Display(Name = "Re-enter new password: ")]
        [MinLength(6, ErrorMessage = "Entered password should be minumum  6 characters")]
        public string PasswordNewConfirm { get; set; } = null!;
    }
}

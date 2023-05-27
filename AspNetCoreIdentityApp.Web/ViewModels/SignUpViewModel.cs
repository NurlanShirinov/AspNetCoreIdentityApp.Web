using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel()
        {
            
        }
        public SignUpViewModel(string userName, string email, string phone, string password, string passwordConfirm)
        {
            UserName = userName;
            Email = email;
            Phone = phone;
            Password = password;
            PasswordConfirm = passwordConfirm;
        }

        [Required(ErrorMessage = "Kullanici ad alanı boş bırakılamaz")]
        [Display(Name ="Kullanici adi : ")]
        public string UserName { get; set; } = null!;


        [EmailAddress(ErrorMessage ="Email formati yanlishdir")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email : ")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "Phone alanı boş bırakılamaz")]
        [Display(Name = "Phone : ")]
        public string Phone { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password alanı boş bırakılamaz")]
        [Display(Name = "Shifre : ")]
        [MinLength(6, ErrorMessage = "Entered password should be minumum  6 characters")]
        public string Password { get; set; } = null!;


        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage ="Shifre ayni deyildir")]
        [Required(ErrorMessage = "Password repeat alanı boş bırakılamaz")]
        [Display(Name = "Shifre Tekrar: ")]
        [MinLength(6, ErrorMessage = "Entered password should be minumum  6 characters")]
        public string PasswordConfirm { get; set; } = null!;
    }
}
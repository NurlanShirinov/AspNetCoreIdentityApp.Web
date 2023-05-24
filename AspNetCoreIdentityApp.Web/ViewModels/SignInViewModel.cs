using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {
            
        }
        public SignInViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [EmailAddress(ErrorMessage = "Email formati yanlishdir")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email : ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password alanı boş bırakılamaz")]
        [Display(Name = "Shifre : ")]
        public string Password { get; set; }

        [Display(Name = "Remember me : ")]
        public bool RememberMe { get; set; }
    }
}

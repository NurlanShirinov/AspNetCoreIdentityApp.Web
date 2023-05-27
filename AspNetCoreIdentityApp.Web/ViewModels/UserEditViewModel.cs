using AspNetCoreIdentityApp.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Kullanici ad alanı boş bırakılamaz")]
        [Display(Name = "Kullanici adi : ")]
        public string UserName { get; set; } = null!;




        [EmailAddress(ErrorMessage = "Email formati yanlishdir")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email : ")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "Phone alanı boş bırakılamaz")]
        [Display(Name = "Phone : ")]
        public string Phone { get; set; } = null!;


        [DataType(DataType.Date)]
        [Display(Name = "BirthDate : ")]
        public DateTime? BirthDate { get; set; }


        [Display(Name = "City : ")]
        public string City { get; set; }


        [Display(Name = "Photo : ")]
        public IFormFile? Picture { get; set; }


        [Display(Name = "Gender : ")]
        public Gender? Gender { get; set; }

    }
}

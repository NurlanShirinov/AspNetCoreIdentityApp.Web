﻿using System.ComponentModel.DataAnnotations;

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
        public string UserName { get; set; }


        [EmailAddress(ErrorMessage ="Email formati yanlishdir")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email : ")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phone alanı boş bırakılamaz")]
        [Display(Name = "Phone : ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password alanı boş bırakılamaz")]
        [Display(Name = "Shifre : ")]
        public string Password { get; set; }


        [Compare(nameof(Password), ErrorMessage ="Shifre ayni deyildir")]
        [Required(ErrorMessage = "Password repeat alanı boş bırakılamaz")]
        [Display(Name = "Shifre Tekrar: ")]
        public string PasswordConfirm { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {
        [Required(ErrorMessage = "Role name cannot be emtpy")]
        [Display(Name = "Role name : ")]
        public string Name { get; set; }
    }
}

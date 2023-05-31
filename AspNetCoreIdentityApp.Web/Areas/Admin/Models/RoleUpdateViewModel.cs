using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public string Id { get; set; } = null!;
        [Required(ErrorMessage = "Role name cannot be emtpy")]
        [Display(Name = "Role name : ")]
        public string Name { get; set; }
    }
}

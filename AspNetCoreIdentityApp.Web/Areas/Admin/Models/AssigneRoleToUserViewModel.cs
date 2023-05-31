namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class AssigneRoleToUserViewModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool Exist { get; set; }
    }
}
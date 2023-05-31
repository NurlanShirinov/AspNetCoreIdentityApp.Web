using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.ClaimProvider
{
    public class UserClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManaManager;

        public UserClaimProvider(UserManager<AppUser> userManaManager)
        {
            _userManaManager = userManaManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {

            var identity = principal.Identity as ClaimsIdentity
            var currentUser = await _userManaManager.FindByNameAsync(principal.Identity);
        }
    }
}

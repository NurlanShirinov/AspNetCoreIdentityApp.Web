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
            var identityUser = principal.Identity as ClaimsIdentity;
            var currentUser = await _userManaManager.FindByNameAsync(identityUser!.Name!);

            if (String.IsNullOrEmpty(currentUser.City))
            {
                return principal;
            }

            if (principal.HasClaim(x => x.Type != "city"))
            {
                Claim cityClaim = new Claim("city", currentUser.City);

                identityUser.AddClaim(cityClaim); // burda userin sherin databasede claim table yazmiriq cunki user artiq bu datani oz tablesinde saxlayir. Biz bunu cookie nin icine add edirik.
            }

            return principal;
        }
    }
}

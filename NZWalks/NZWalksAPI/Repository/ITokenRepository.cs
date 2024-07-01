using Microsoft.AspNetCore.Identity;

namespace NZWalksAPI.Repository
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser identityUser, List<string> roles);
    }
}

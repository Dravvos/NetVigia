using Microsoft.AspNetCore.Identity;
using NetVigia.Identity.Models;

namespace NetVigia.Identity.Service
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager);
    }
}

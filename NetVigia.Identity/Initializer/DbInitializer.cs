using Microsoft.AspNetCore.Identity;
using NetVigia.Identity.Configuration;
using NetVigia.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NetVigia.Identity.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(IdentityConfiguration.Admin).Result != null) return;

            _roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

            ApplicationUser admin = new ApplicationUser()
            {
                Nome = "Daniel",
                Email = "daniddias53@gmail.com",
                EmailConfirmed = true,
                Sobrenome = "Admin",
                UserName = "Admin-DD",
                AceitouOsTermosDeUsoPrivacidade = true
            };

            _userManager.CreateAsync(admin, "*Igowallah2024*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();

            var adminClaims = _userManager.AddClaimsAsync(admin, new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Name,$"{admin.Nome} {admin.Sobrenome}"),
                new Claim(JwtRegisteredClaimNames.GivenName,$"{admin.Nome}"),
                new Claim(JwtRegisteredClaimNames.FamilyName,$"{admin.Sobrenome}"),
                new Claim(ClaimTypes.Role,IdentityConfiguration.Admin)
            }).Result;

            ApplicationUser client = new ApplicationUser()
            {
                Nome = "Daniel",
                Email = "danieldiasdasquebrada@gmail.com",
                EmailConfirmed = true,
                Sobrenome = "Cliente",
                UserName = "Client-DD",
                AceitouOsTermosDeUsoPrivacidade = true
            };

            _userManager.CreateAsync(client, "*Igowallah2024*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(client, IdentityConfiguration.Client).GetAwaiter().GetResult();

            var clientClaims = _userManager.AddClaimsAsync(client, new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Name,$"{client.Nome} {client.Sobrenome}"),
                new Claim(JwtRegisteredClaimNames.GivenName,$"{client.Nome}"),
                new Claim(JwtRegisteredClaimNames.FamilyName,$"{client.Sobrenome}"),
                new Claim(ClaimTypes.Role,IdentityConfiguration.Client)
            }).Result;
        }
    }
}

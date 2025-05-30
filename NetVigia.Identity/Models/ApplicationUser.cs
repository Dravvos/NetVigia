using Microsoft.AspNetCore.Identity;

namespace NetVigia.Identity.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public bool AceitouOsTermosDeUsoPrivacidade { get; set; }
    }
}

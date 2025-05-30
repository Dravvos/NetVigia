using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NetVigia.Identity.Models
{
    public class AuthContext:IdentityDbContext<ApplicationUser>
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {

        }

        public AuthContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = Environment.GetEnvironmentVariable("NetVigiaCon");
            optionsBuilder.UseNpgsql(config);
        }
    }
}

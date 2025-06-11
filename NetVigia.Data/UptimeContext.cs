using Microsoft.EntityFrameworkCore;
using NetVigia.Data.Models;

namespace NetVigia.Data
{
    public class UptimeContext : DbContext
    {
        public UptimeContext()
        {

        }
        public UptimeContext(DbContextOptions<UptimeContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var con = Environment.GetEnvironmentVariable("NetVigiaCon");
            optionsBuilder.UseNpgsql(con);
        }


        public DbSet<ServerModel> Servers { get; set; }
    }
}

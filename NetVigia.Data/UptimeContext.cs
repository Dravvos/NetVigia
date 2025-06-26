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

        public DbSet<IntegrationUserModel> Integrations { get; set; }
        public DbSet<MaintenanceModel> Maintenances { get; set; }
        public DbSet<MaintenanceServerModel> MaintenanceServers { get; set; }
        public DbSet<ServerModel> Servers { get; set; }
        public DbSet<TabelaGeralModel> TabelaGeral { get; set; }
        public DbSet<TabelaGeralItemModel> TabelaGeralItem { get; set; }
    }
}

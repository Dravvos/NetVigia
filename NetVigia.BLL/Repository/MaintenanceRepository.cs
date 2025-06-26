using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.Data;
using NetVigia.Data.Models;
using NetVigia.DTO;

namespace NetVigia.BLL.Repository
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly UptimeContext con;

        public MaintenanceRepository(UptimeContext con)
        {
            this.con = con;
        }

        public async Task CreateMaintenanceAsync(MaintenanceDTO dto)
        {
            var model = Map<MaintenanceModel>.Convert(dto);
            await con.Maintenances.AddAsync(model);
            foreach (var server in model.Servers)
            {
                await con.MaintenanceServers.AddAsync(new MaintenanceServerModel
                {
                    DataInclusao = model.DataInclusao,
                    UsuarioInclusao = model.UsuarioInclusao,
                    Id = Guid.NewGuid(),
                    MaintenanceId = model.Id,
                    ServerId = server.Id
                });
            }
            await con.SaveChangesAsync();
        }

        public Task DeleteMaintenanceAsync(Guid id)
        {
            var maintenance = con.Maintenances.FirstOrDefault(x => x.Id == id);
            con.MaintenanceServers.RemoveRange(con.MaintenanceServers.Where(x => x.MaintenanceId == id));
            con.Maintenances.Remove(maintenance);
            return con.SaveChangesAsync();
        }

        public async Task<IEnumerable<MaintenanceDTO>> GetActiveMaintenanceWindowsAsync()
        {
            var now = DateTime.UtcNow;
            var model = await con.Maintenances.Where(x => now >= x.StartDate && now <= x.EndDate).ToListAsync();
            return Map<List<MaintenanceDTO>>.Convert(model);
        }

        public async Task<List<MaintenanceDTO>> GetAllMaintenanceAsync(Guid userId)
        {
            var maintenances = await con.Maintenances
                .Where(x => x.UserId == userId).Include(x => x.Servers)
                .Select(x => Map<MaintenanceDTO>.Convert(x))
                .ToListAsync();
            return maintenances;
        }

        public async Task<MaintenanceDTO> GetMaintenanceByIdAsync(Guid id)
        {
            var model = await con.Maintenances.Include(x => x.Servers).FirstOrDefaultAsync(x => x.Id == id);
            return Map<MaintenanceDTO>.Convert(model);
        }

        public async Task<IEnumerable<MaintenanceDTO>> GetUpcomingMaintenanceWindowsAsync()
        {
            var model = await con.Maintenances.Where(x => x.StartDate > DateTime.UtcNow).ToListAsync();
            return Map<List<MaintenanceDTO>>.Convert(model);
        }

        public async Task UpdateMaintenanceAsync(MaintenanceDTO dto)
        {
            var model = await con.Maintenances.FirstAsync(x => x.Id == dto.Id);

            model.StartDate = dto.StartDate;
            model.EndDate = dto.EndDate;
            model.Title = dto.Title;
            model.DataAlteracao = dto.DataAlteracao;
            model.UsuarioAlteracao = dto.UsuarioAlteracao;

            var servers = await con.MaintenanceServers
                .Where(x => x.MaintenanceId == dto.Id).ToListAsync();

            var serversToRemove = servers.Where(x => !dto.Servers.Any(s => s.Id == x.ServerId)).ToList();

            foreach (var server in serversToRemove)
                con.MaintenanceServers.Remove(server);

            var serversToAdd = dto.Servers.Where(s => !servers.Any(x => x.ServerId == s.Id)).ToList();
            foreach (var server in serversToAdd)
            {
                await con.MaintenanceServers.AddAsync(new MaintenanceServerModel
                {
                    DataInclusao = model.DataInclusao,
                    UsuarioInclusao = model.UsuarioInclusao,
                    Id = Guid.NewGuid(),
                    MaintenanceId = model.Id,
                    ServerId = server.Id.GetValueOrDefault()
                });
            }

            await con.SaveChangesAsync();
        }
    }
}

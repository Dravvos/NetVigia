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
            model.Servers.Clear();
            model.StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
            model.EndDate = DateTime.SpecifyKind(model.EndDate, DateTimeKind.Utc);
            await con.Maintenances.AddAsync(model);
            await con.SaveChangesAsync();
            foreach (var server in dto.Servers)
            {
                var msModel = new MaintenanceServerModel
                {
                    DataInclusao = DateTime.UtcNow,
                    UsuarioInclusao = model.UsuarioInclusao,
                    Id = Guid.NewGuid(),
                    MaintenanceId = model.Id,
                    ServerId = server.Id.GetValueOrDefault()
                };
                await con.MaintenanceServers.AddAsync(msModel);
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

            foreach (var item in model)
            {
                item.Servers = await con.MaintenanceServers
                    .Where(x => x.MaintenanceId == item.Id)
                    .Select(x => x.Server)
                    .ToListAsync();
            }

            return Map<List<MaintenanceDTO>>.Convert(model);
        }

        public async Task<List<MaintenanceDTO>> GetAllMaintenanceAsync(Guid userId)
        {
            var model = await con.Maintenances.Where(x => x.UserId == userId).Include(x => x.Servers).ToListAsync();


            foreach (var item in model)
            {
                var listAux = await con.MaintenanceServers.Where(x => x.MaintenanceId == item.Id).ToListAsync();
                var serversIds = listAux.Select(x => x.ServerId).Distinct().ToList();
                item.Servers = await con.Servers.Where(x => serversIds.Contains(x.Id)).Include(x => x.MonitoringType).Include(x => x.HTTPMethod).ToListAsync();
            }

            var dto = Map<List<MaintenanceDTO>>.Convert(model);

            return dto;
        }

        public async Task<List<MaintenanceDTO>> GetByDateAsync(Guid? serverId, Guid userId, DateTime startDate, DateTime endDate)
        {
            var model = await con.Maintenances.Where(x => x.UserId == userId && x.StartDate >= startDate && x.EndDate <= endDate)
                                                .Include(x => x.Servers).ToListAsync();
            if (serverId.HasValue && serverId.Value != Guid.Empty)
            {
                model = model.Where(x => x.Servers.Any(s => s.Id == serverId.Value)).ToList();
            }

            foreach (var item in model)
            {
                var listAux = await con.MaintenanceServers.Where(x => x.MaintenanceId == item.Id).ToListAsync();
                var serversIds = listAux.Select(x => x.ServerId).Distinct().ToList();
                item.Servers = await con.Servers.Where(x => serversIds.Contains(x.Id)).Include(x => x.MonitoringType).Include(x => x.HTTPMethod).ToListAsync();
            }

            var dto = Map<List<MaintenanceDTO>>.Convert(model);

            return dto;
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

            model.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
            model.EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);
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

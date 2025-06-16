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
    public class ServerRepository : IServerRepository
    {

        private readonly UptimeContext con;

        public ServerRepository(UptimeContext con)
        {
            this.con = con;
        }

        public async Task AddAsync(ServerDTO dto)
        {
            var model = Map<ServerModel>.Convert(dto);
            await con.Servers.AddAsync(model);
            await con.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var model = await con.Servers.FirstAsync(x => x.Id == id);
            con.Servers.Remove(model);
            await con.SaveChangesAsync();
        }

        public async Task<List<ServerDTO>> GetActiveWebsitesAsync()
        {
            var model = await con.Servers.Where(x => x.Active).Include(x => x.MonitoringType).ToListAsync();
            return Map<List<ServerDTO>>.Convert(model);
        }

        public async Task<List<ServerDTO>> GetAllAsync(Guid userId)
        {
            var model = await con.Servers.Where(x => x.UserId == userId).Include(x => x.MonitoringType).ToListAsync();
            return Map<List<ServerDTO>>.Convert(model);
        }

        public async Task<ServerDTO> GetByIdAsync(Guid id)
        {
            var model = await con.Servers.Include(x => x.MonitoringType).FirstOrDefaultAsync(x => x.Id == id);
            return Map<ServerDTO>.Convert(model);
        }

        public async Task<List<ServerDTO>> GetByNomeAsync(Guid userId, string nome)
        {
            var model = await con.Servers.Where(x => x.UserId == userId && x.URL.ToLower().Contains(nome.ToLower())).Include(x => x.MonitoringType).ToListAsync();
            return Map<List<ServerDTO>>.Convert(model);
        }

        public async Task<ServerDTO> GetByUrl(string? url)
        {
            var model = await con.Servers.Include(x => x.MonitoringType).FirstOrDefaultAsync(x => x.URL == url);
            return Map<ServerDTO>.Convert(model);
        }

        public async Task UpdateAsync(ServerDTO dto)
        {
            var model = await con.Servers.FirstAsync(x => x.Id == dto.Id);

            model.ExpectedStatusCode = dto.ExpectedStatusCode;
            model.URL = dto.URL;
            model.CheckIntervalSeconds = dto.CheckIntervalSeconds;
            model.Active = dto.Active;
            model.TimeoutInSeconds = dto.TimeoutInSeconds;
            model.ExpectedContent = dto.ExpectedContent;
            model.Name = dto.Name;
            model.IdTGMonitoringType = dto.IdTGMonitoringType;
            model.ExpectedStatusCode = dto.ExpectedStatusCode;
            model.UsuarioAlteracao = dto.UsuarioAlteracao;
            model.DataAlteracao = dto.DataAlteracao;

            await con.SaveChangesAsync();
        }
    }
}

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
    public class IntegrationRepository : IIntegrationRepository
    {
        private readonly UptimeContext con;

        public IntegrationRepository(UptimeContext con)
        {
            this.con = con;
        }

        public async Task CreateAsync(IntegrationDTO dto)
        {
            var model = Map<IntegrationUserModel>.Convert(dto);
            await con.Integrations.AddAsync(model);
            await con.SaveChangesAsync();

            foreach(var item in dto.Servers)
            {
                var integrationServer = new IntegrationServerModel
                {
                    DataInclusao = DateTime.UtcNow,
                    UsuarioInclusao = model.UsuarioInclusao,
                    Id = Guid.NewGuid(),
                    IntegrationId = model.Id,
                    ServerId = item.Id.GetValueOrDefault()
                };
                await con.IntegrationServers.AddAsync(integrationServer);
            }
            await con.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await con.Integrations
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<IntegrationDTO> GetByIdAsync(Guid id)
        {
            var model = await con.Integrations.Include(x => x.IntegrationMethod).Include(x => x.SendNotification)
                .Include(x => x.TypeNotification).FirstOrDefaultAsync(x => x.Id == id);

            var dto = Map<IntegrationDTO>.Convert(model);

            var integrationServer = await con.IntegrationServers.Where(x => x.IntegrationId == dto.Id)
                .Include(x => x.Server).Select(x => x.Server).ToListAsync();

            dto.Servers = Map<List<ServerDTO>>.Convert(integrationServer);

            return dto;
        }

        public async Task<List<IntegrationDTO>> GetByUserAsync(Guid userId)
        {
            var model = await con.Integrations.Where(x => x.UserId == userId).Include(x => x.IntegrationMethod)
                .Include(x => x.SendNotification).Include(x => x.TypeNotification).ToListAsync();

            var dto = Map<List<IntegrationDTO>>.Convert(model);
            foreach (var item in dto)
            {
                var integrationServer = await con.IntegrationServers.Where(x => x.IntegrationId == item.Id)
                    .Include(x => x.Server).Select(x => x.Server).ToListAsync();

                item.Servers = Map<List<ServerDTO>>.Convert(integrationServer);
            }

            return dto;
        }

        public async Task UpdateAsync(IntegrationDTO dto)
        {
            var model = await con.Integrations.FirstAsync(x => x.Id == dto.Id);

            model.IdTGIntegrationMethod = dto.IdTGIntegrationMethod;
            model.IntegrationName = dto.IntegrationName;
            model.IntegrationEndpoint = dto.IntegrationEndpoint;
            model.IdTGSendNotification = dto.IdTGSendNotification;
            model.IdTGTypeNotification = dto.IdTGTypeNotification;
            model.Active = dto.Active;
            model.DataAlteracao = dto.DataAlteracao;
            model.UsuarioAlteracao = dto.UsuarioAlteracao;

            foreach (var item in dto.Servers)
            {
                var integrationServer = await con.IntegrationServers.FirstAsync(x=>x.ServerId == item.Id && x.IntegrationId == dto.Id);
                integrationServer.ServerId = item.Id.GetValueOrDefault();
                integrationServer.IntegrationId = dto.Id.GetValueOrDefault();
                integrationServer.DataAlteracao = dto.DataAlteracao;
                integrationServer.UsuarioAlteracao = dto.UsuarioAlteracao;
            }

            await con.SaveChangesAsync();
        }
    }
}

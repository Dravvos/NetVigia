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
        }

        public async Task DeleteAsync(Guid id)
        {
            await con.Integrations
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<IntegrationDTO> GetByIdAsync(Guid id)
        {
            var model = await con.Integrations.Include(x => x.IntegrationMethod).Include(x=>x.SendNotification)
                .Include(x=>x.TypeNotification).FirstOrDefaultAsync(x => x.Id == id);

            return Map<IntegrationDTO>.Convert(model);
        }

        public async Task<List<IntegrationDTO>> GetByUserAsync(Guid userId)
        {
            var model = await con.Integrations.Where(x => x.UserId == userId).Include(x => x.IntegrationMethod)
                .Include(x=>x.SendNotification).Include(x => x.TypeNotification).ToListAsync();

            return Map<List<IntegrationDTO>>.Convert(model);
        }

        public async Task UpdateAsync(IntegrationDTO dto)
        {
            var model = await con.Integrations.FirstAsync(x => x.Id == dto.Id);

            model.IdTGIntegrationMethod = dto.IdTGIntegrationMethod;
            model.IntegrationName = dto.IntegrationName;
            model.IntegrationEndpoint = dto.IntegrationEndpoint;
            model.DataAlteracao = dto.DataAlteracao;
            model.UsuarioAlteracao = dto.UsuarioAlteracao;

            await con.SaveChangesAsync();
        }
    }
}

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
    public class TabelaGeralItemRepository : ITabelaGeralItemRepository
    {
        private readonly UptimeContext con;

        public TabelaGeralItemRepository(UptimeContext con)
        {
            this.con = con;
        }


        public async Task<TabelaGeralItemDTO> AddAsync(TabelaGeralItemDTO item)
        {
            var tabelaGeralItem = Map<TabelaGeralItemModel>.Convert(item);
            await con.TabelaGeralItem.AddAsync(tabelaGeralItem);
            await con.SaveChangesAsync();
            return Map<TabelaGeralItemDTO>.Convert(tabelaGeralItem);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var tabelaGeralItem = await con.TabelaGeralItem.FirstOrDefaultAsync(x => x.Id == id);
                if (tabelaGeralItem == null)
                    return false;
                con.TabelaGeralItem.Remove(tabelaGeralItem);
                await con.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<TabelaGeralItemDTO>> GetAllAsync()
        {
            var ItensTabelasGerais = await con.TabelaGeralItem.Include(x => x.TabelaGeral).ToListAsync();
            return Map<List<TabelaGeralItemDTO>>.Convert(ItensTabelasGerais);
        }

        public async Task<List<TabelaGeralItemDTO>> GetAllItemsAsync(Guid? tabelaGeralId)
        {
            var ItensTabelasGerais = new List<TabelaGeralItemModel>();
            if (tabelaGeralId != null)
                ItensTabelasGerais = await con.TabelaGeralItem.Where(x => x.TabelaGeralId == tabelaGeralId.Value).Include(x => x.TabelaGeral).ToListAsync();
            else
                ItensTabelasGerais = await con.TabelaGeralItem.Include(x => x.TabelaGeral).ToListAsync();

            return Map<List<TabelaGeralItemDTO>>.Convert(ItensTabelasGerais);
        }

        public async Task<TabelaGeralItemDTO> GetByIdAsync(Guid id)
        {
            var tabelaGeralItem = await con.TabelaGeralItem.Include(x => x.TabelaGeral).FirstOrDefaultAsync(x => x.Id == id);
            return Map<TabelaGeralItemDTO>.Convert(tabelaGeralItem);
        }

        public async Task<TabelaGeralItemDTO> GetBySiglaAsync(Guid tabelaGeralId, string sigla)
        {
            var tabelaGeralItem = await con.TabelaGeralItem.Include(x => x.TabelaGeral).FirstOrDefaultAsync(x => x.TabelaGeralId == tabelaGeralId && x.Sigla == sigla);
            return Map<TabelaGeralItemDTO>.Convert(tabelaGeralItem);
        }

        public async Task<TabelaGeralItemDTO> UpdateAsync(TabelaGeralItemDTO item)
        {
            var model = await con.TabelaGeralItem.FirstAsync(x => x.Id == item.Id);

            model.Descricao = item.Descricao;
            model.DataAlteracao = DateTime.UtcNow.ToUniversalTime();
            model.Sigla = item.Sigla.ToUpper();

            await con.SaveChangesAsync();
            return Map<TabelaGeralItemDTO>.Convert(model);
        }
    }
}

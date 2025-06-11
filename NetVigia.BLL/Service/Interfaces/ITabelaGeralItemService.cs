using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Service.Interfaces
{
    public interface ITabelaGeralItemService
    {
        Task<TabelaGeralItemDTO> GetByIdAsync(Guid id);
        Task<TabelaGeralItemDTO> GetBySiglaAsync(Guid tabelaGeralId, string sigla);
        Task<List<TabelaGeralItemDTO>> GetAllItemsAsync(Guid? tabelaGeralId);
        Task AddAsync(TabelaGeralItemDTO model);
        Task UpdateAsync(TabelaGeralItemDTO model);
        Task DeleteAsync(Guid id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Repository.Interfaces
{
    public interface ITabelaGeralItemRepository
    {
        Task<TabelaGeralItemDTO> GetByIdAsync(Guid id);
        Task<TabelaGeralItemDTO> GetBySiglaAsync(Guid tabelaGeralId, string sigla);
        Task<List<TabelaGeralItemDTO>> GetAllAsync();
        Task<List<TabelaGeralItemDTO>> GetAllItemsAsync(Guid? tabelaGeralId);
        Task<TabelaGeralItemDTO> AddAsync(TabelaGeralItemDTO item);
        Task<TabelaGeralItemDTO> UpdateAsync(TabelaGeralItemDTO item);
        Task<bool> DeleteAsync(Guid id);
    }
}

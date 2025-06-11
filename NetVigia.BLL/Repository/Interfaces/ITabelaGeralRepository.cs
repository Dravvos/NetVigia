using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Repository.Interfaces
{
    public interface ITabelaGeralRepository
    {
        Task<TabelaGeralDTO> GetByIdAsync(Guid id);
        Task<TabelaGeralDTO> GetByNomeAsync(string nome);
        Task<List<TabelaGeralDTO>> GetAllAsync();
        Task<TabelaGeralDTO> AddAsync(TabelaGeralDTO dto);
        Task<TabelaGeralDTO> UpdateAsync(TabelaGeralDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Service.Interfaces
{
    public interface IServerService
    {
        Task<ServerDTO> GetById(Guid id);
        Task<List<ServerDTO>> GetByNomeAsync(Guid userId, string nome);
        Task<List<ServerDTO>> GetAllAsync(Guid userId);
        Task AddAsync(ServerDTO dto);
        Task UpdateAsync(ServerDTO dto);
        Task DeleteAsync(Guid id);
    }
}

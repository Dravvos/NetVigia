using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Repository.Interfaces
{
    public interface IIntegrationRepository
    {
        Task<List<IntegrationDTO>> GetByUserAsync(Guid userId);
        Task<IntegrationDTO> GetByIdAsync(Guid id);
        Task CreateAsync(IntegrationDTO dto);
        Task UpdateAsync(IntegrationDTO dto);
        Task DeleteAsync(Guid id);
    }
}

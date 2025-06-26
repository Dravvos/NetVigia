using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Service.Interfaces
{
    public interface IIntegrationService
    {
        Task<List<IntegrationDTO>> GetByUserAsync(Guid userId);
        Task CreateAsync(IntegrationDTO dto);
        Task UpdateAsync(IntegrationDTO dto);
        Task DeleteAsync(Guid id);
        Task SendNotificationAsync(string endpoint, object data);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Repository.Interfaces
{
    public interface IMaintenanceRepository
    {
        Task<List<MaintenanceDTO>> GetAllMaintenanceAsync(Guid userId);
        Task<MaintenanceDTO> GetMaintenanceByIdAsync(Guid id);
        Task CreateMaintenanceAsync(MaintenanceDTO maintenance);
        Task UpdateMaintenanceAsync(MaintenanceDTO maintenance);
        Task DeleteMaintenanceAsync(Guid id);

        Task<IEnumerable<MaintenanceDTO>> GetActiveMaintenanceWindowsAsync();
        Task<IEnumerable<MaintenanceDTO>> GetUpcomingMaintenanceWindowsAsync();
    }
}

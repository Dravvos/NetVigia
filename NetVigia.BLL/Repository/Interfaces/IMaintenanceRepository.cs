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
        Task<List<MaintenanceDTO>> GetByDateAsync(List<Guid> serverIds, DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetGroupedByDateAsync(Guid groupingType, List<Guid> serverIds, DateTime startDate, DateTime endDate);
        Task<MaintenanceDTO> GetMaintenanceByIdAsync(Guid id);
        Task<TimeSpan> GetTotalMaintenanceDuration(DateTime startDate, DateTime endDate, List<Guid> serverIds);
        Task<TimeSpan> GetAverageMaintenanceDuration(DateTime startDate, DateTime endDate, List<Guid> serverIds);
        Task CreateMaintenanceAsync(MaintenanceDTO maintenance);
        Task UpdateMaintenanceAsync(MaintenanceDTO maintenance);
        Task DeleteMaintenanceAsync(Guid id);

        Task<IEnumerable<MaintenanceDTO>> GetActiveMaintenanceWindowsAsync();
        Task<IEnumerable<MaintenanceDTO>> GetUpcomingMaintenanceWindowsAsync();
    }
}

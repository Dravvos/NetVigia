using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Service.Interfaces
{
    public interface IMaintenanceService
    {
        Task<List<MaintenanceDTO>> GetAllMaintenanceAsync(Guid userId);
        Task<List<MaintenanceDTO>> GetByDateAsync(List<Guid> serverIds, DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetGroupedByDateAsync(Guid groupingType, List<Guid> serverIds, DateTime startDate, DateTime endDate);
        Task<TimeSpan> GetTotalMaintenanceDuration(DateTime startDate, DateTime endDate, List<Guid> serverIds);
        Task<TimeSpan> GetAverageMaintenanceDuration(DateTime startDate, DateTime endDate, List<Guid> serverIds);
        Task CreateMaintenanceAsync(MaintenanceDTO maintenance);
        Task UpdateMaintenanceAsync(MaintenanceDTO maintenance);
        Task DeleteMaintenanceAsync(Guid id);
        
        Task<bool> IsUnderMaintenanceAsync(Guid serverId);
        Task<bool> IsUnderMaintenanceAsync(ServerDTO server);
    }
}

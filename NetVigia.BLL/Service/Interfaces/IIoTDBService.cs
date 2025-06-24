using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.BLL.Service.Interfaces
{
    public interface IIoTDBService
    {
        Task<double> GetUptimePercentageAsync(Guid serverId, DateTime startDate, DateTime? endDate);
        Task<bool> Insert(CheckDTO check);
        Task<List<CheckDTO>> ListChecks(Guid serverId, DateTime startDate, DateTime? endDate);
        Task<List<CheckDTO>> ListFailedChecks(Guid serverId, DateTime startDate, DateTime? endDate);
        Task<List<CheckDTO>> GetFailedChecksByDate(Guid serverId, DateTime startDate, DateTime? endDate);
        Task<double> GetAverageResponseTime(Guid serverId, DateTime startDate, DateTime? endDate);
        Task<List<CheckDTO>> GetAverageResponseTimeByDate(Guid serverId, DateTime startDate, DateTime? endDate);
    }
}

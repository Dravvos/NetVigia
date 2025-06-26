using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.DTO;

namespace NetVigia.Workers.Workers.Interfaces
{
    public interface ICheckScheduler
    {
        IReadOnlyCollection<Guid> GetScheduledWebsiteIds();
        void ScheduleWebsiteCheck(ServerDTO website);
        void UnscheduleWebsiteCheck(Guid websiteId);
        void UpdateWebsiteCheckSchedule(ServerDTO website);
        bool IsScheduled(Guid websiteId);
    }
}

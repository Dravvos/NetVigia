using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.DTO;

namespace NetVigia.BLL.Query
{
    public record GetMaintenanceByDateQuery(Guid? serverId, Guid userId, DateTime startDate, DateTime endDate):IRequest<List<MaintenanceDTO>>;
}

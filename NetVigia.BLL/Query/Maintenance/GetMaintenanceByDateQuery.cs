using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.DTO;

namespace NetVigia.BLL.Query.Maintenance
{
    public record GetMaintenanceByDateQuery(List<Guid> serverIds, DateTime startDate, DateTime endDate):IRequest<List<MaintenanceDTO>>;
}

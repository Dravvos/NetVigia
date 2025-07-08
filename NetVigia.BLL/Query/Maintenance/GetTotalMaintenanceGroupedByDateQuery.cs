using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NetVigia.BLL.Query.Maintenance
{
    public record GetTotalMaintenanceGroupedByDateQuery(Guid groupingType, DateTime startDate, DateTime endDate, List<Guid> serverIds):IRequest<Dictionary<string, int>>;
    
    
}

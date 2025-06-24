using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.Query.Checks
{
    public record GetAverageResponseTimeQuery(Guid serverId, DateTime startDate, DateTime? endDate):IRequest<double>;
}

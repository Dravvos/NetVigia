using MediatR;
using NetVigia.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.Query.Checks
{
    public record GetAverageResponseTimeByDateQuery(Guid serverId, DateTime startDate, DateTime? endDate) : IRequest<List<CheckDTO>>;

}

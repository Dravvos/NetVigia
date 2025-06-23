using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.DTO;

namespace NetVigia.BLL.Query.Checks
{
    public record GetFailedChecksByDateQuery(Guid serverId, DateTime startDate, DateTime? endDate) :IRequest<List<CheckDTO>>;
}

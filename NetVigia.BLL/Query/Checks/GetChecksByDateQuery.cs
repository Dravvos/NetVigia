using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.DTO;

namespace NetVigia.BLL.Query.Checks
{
    public record GetChecksByDateQuery(DateTime startDate, DateTime endDate, Guid serverId): IRequest<List<CheckDTO>>;
}

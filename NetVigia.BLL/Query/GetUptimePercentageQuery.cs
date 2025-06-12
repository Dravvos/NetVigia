using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NetVigia.BLL.Query
{
    public record GetUptimePercentageQuery(Guid serverId, TimeSpan period) : IRequest<double>;
}

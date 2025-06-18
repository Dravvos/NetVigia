using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.Query
{
    public record GetAverageResponseTimeQuery(Guid serverId, TimeSpan period):IRequest<double>;
}

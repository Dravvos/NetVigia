using MediatR;
using NetVigia.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.Query.Checks
{
    public record GetTop5DowntimeServersQuery(Guid userId, DateTime startDate, DateTime endDate):IRequest<List<ServerDTO>>;
}

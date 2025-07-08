using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NetVigia.BLL.Query.Checks
{
    public record GetUptimePercentageOfAllServersQuery(Guid userId, DateTime startDate, DateTime endDate) : IRequest<double>;
}

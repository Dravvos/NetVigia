using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Query.Checks;
using NetVigia.BLL.Service.Interfaces;

namespace NetVigia.BLL.QueryHandler.Checks
{
    public class GetUptimePercentageOfAllServersQueryHandler : IRequestHandler<GetUptimePercentageOfAllServersQuery, double>
    {
        private readonly IIoTDBService _checksService;
        public GetUptimePercentageOfAllServersQueryHandler(IIoTDBService checksService)
        {
            _checksService = checksService;
        }

        public async Task<double> Handle(GetUptimePercentageOfAllServersQuery request, CancellationToken cancellationToken)
        {
            return await _checksService.GetUptimePercentageOfAllServersAsync(request.userId, request.startDate, request.endDate);
        }
    }
}

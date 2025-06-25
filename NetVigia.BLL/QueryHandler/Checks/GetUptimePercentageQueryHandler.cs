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
    public class GetUptimePercentageQueryHandler : IRequestHandler<GetUptimePercentageQuery, double>
    {
        private readonly IIoTDBService _ioTDBService;

        public GetUptimePercentageQueryHandler(IIoTDBService ioTDBService)
        {
            _ioTDBService = ioTDBService;
        }

        public Task<double> Handle(GetUptimePercentageQuery request, CancellationToken cancellationToken)
        {
            return _ioTDBService.GetUptimePercentageAsync(request.serverId, request.startDate, request.endDate);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NetVigia.BLL.Query;
using NetVigia.BLL.Service.Interfaces;

namespace NetVigia.BLL.QueryHandler
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
            if (request.period <= TimeSpan.Zero)
                throw new ArgumentException("Period must be greater than zero.", nameof(request.period));
            return _ioTDBService.GetUptimePercentageAsync(request.serverId, request.period);
        }
    }
}

using MediatR;
using NetVigia.BLL.Query.Checks;
using NetVigia.BLL.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetVigia.BLL.QueryHandler.Checks
{
    public class GetAverageResponseTimeQueryHandler : IRequestHandler<GetAverageResponseTimeQuery, double>
    {
        private readonly IIoTDBService _service;

        public GetAverageResponseTimeQueryHandler(IIoTDBService service)
        {
            _service = service;
        }

        public async Task<double> Handle(GetAverageResponseTimeQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetAverageResponseTime(request.serverId, request.startDate, request.endDate);
        }
    }
}
